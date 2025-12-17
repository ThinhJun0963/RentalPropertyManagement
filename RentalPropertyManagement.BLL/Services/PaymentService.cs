using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using RentalPropertyManagement.BLL.Utils;
using RentalPropertyManagement.DAL.Entities;
using RentalPropertyManagement.DAL.Interfaces;

namespace RentalPropertyManagement.BLL.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public PaymentService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<string> CreateVNPayPaymentUrlAsync(VNPayPaymentDTO dto)
        {
            // Get payment invoice
            var invoice = await _unitOfWork.PaymentInvoices.GetByIdAsync(dto.PaymentInvoiceId);
            if (invoice == null)
                throw new Exception("Payment invoice not found");

            var tmnCode = _configuration["VNPay:TmnCode"];
            var hashSecret = _configuration["VNPay:HashSecret"];
            var apiUrl = _configuration["VNPay:ApiUrl"];

            // Generate unique TxnRef
            var now = DateTime.Now;
            var txnRef = $"{now:yyyyMMddHHmmss}{(invoice.Id % 1000000):000000}".Substring(0, 20);

            // Use official VNPay library
            var vnpay = new VNPayUtil();
            
            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", tmnCode);
            vnpay.AddRequestData("vnp_Amount", ((long)(invoice.Amount * 100)).ToString());
            vnpay.AddRequestData("vnp_CreateDate", now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", dto.IpAddress ?? "127.0.0.1");
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", RemoveVietnameseDiacritics($"Thanh toan hoa don {invoice.Id}"));
            vnpay.AddRequestData("vnp_OrderType", "billpayment");
            vnpay.AddRequestData("vnp_ReturnUrl", dto.ReturnUrl);
            vnpay.AddRequestData("vnp_TxnRef", txnRef);
            vnpay.AddRequestData("vnp_ExpireDate", now.AddMinutes(15).ToString("yyyyMMddHHmmss"));

            // Create payment URL with signature
            string paymentUrl = vnpay.CreateRequestUrl(apiUrl, hashSecret);
            return paymentUrl;
        }

        // Helper method to remove Vietnamese diacritics
        private string RemoveVietnameseDiacritics(string text)
        {
            var normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
            var stringBuilder = new System.Text.StringBuilder();
            
            foreach (char c in normalizedString)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC);
        }

        public async Task<PaymentDTO> ProcessVNPayCallbackAsync(VNPayCallbackDTO dto)
        {
            var hashSecret = _configuration["VNPay:HashSecret"];

            // Use official VNPay library for validation
            var vnpay = new VNPayUtil();
            
            // Add all response data
            vnpay.AddResponseData("vnp_Amount", dto.vnp_Amount);
            vnpay.AddResponseData("vnp_BankCode", dto.vnp_BankCode);
            vnpay.AddResponseData("vnp_BankTranNo", dto.vnp_BankTranNo);
            vnpay.AddResponseData("vnp_CardType", dto.vnp_CardType);
            vnpay.AddResponseData("vnp_OrderInfo", dto.vnp_OrderInfo);
            vnpay.AddResponseData("vnp_PayDate", dto.vnp_PayDate);
            vnpay.AddResponseData("vnp_ResponseCode", dto.vnp_ResponseCode);
            vnpay.AddResponseData("vnp_TmnCode", dto.vnp_TmnCode);
            vnpay.AddResponseData("vnp_TransactionNo", dto.vnp_TransactionNo);
            vnpay.AddResponseData("vnp_TransactionStatus", dto.vnp_TransactionStatus);
            vnpay.AddResponseData("vnp_TxnRef", dto.vnp_TxnRef);

            // Validate signature
            if (!vnpay.ValidateSignature(dto.vnp_SecureHash, hashSecret))
                throw new Exception("Invalid signature");

            // Get payment invoice - extract ID from TxnRef
            // TxnRef format: yyyyMMddHHmmss (14 chars) + last 6 digits of invoice ID (6 chars)
            // Example: "20251217110512000001" -> Invoice ID = 1
            int invoiceId;
            if (dto.vnp_TxnRef.Length >= 6)
            {
                // Extract last 6 characters as invoice ID
                string invoiceIdStr = dto.vnp_TxnRef.Substring(dto.vnp_TxnRef.Length - 6);
                if (!int.TryParse(invoiceIdStr, out invoiceId) || invoiceId == 0)
                    throw new Exception("Invalid transaction reference");
            }
            else
            {
                throw new Exception("Invalid transaction reference");
            }

            var invoice = await _unitOfWork.PaymentInvoices.GetByIdAsync(invoiceId);
            if (invoice == null)
                throw new Exception("Payment invoice not found");

            // Create payment record
            var payment = new Payment
            {
                PaymentInvoiceId = invoiceId,
                TenantId = invoice.TenantId,
                PaymentDate = DateTime.Now,
                Amount = long.Parse(dto.vnp_Amount) / 100m,
                PaymentMethod = "VNPay",
                TransactionId = dto.vnp_TransactionNo,
                ResponseCode = dto.vnp_ResponseCode,
                ResponseMessage = dto.vnp_TransactionStatus,
                Status = dto.vnp_ResponseCode == "00" 
                    ? DAL.Enums.PaymentStatus.Completed 
                    : DAL.Enums.PaymentStatus.Failed
            };

            _unitOfWork.Payments.Add(payment);

            // Update invoice status
            if (dto.vnp_ResponseCode == "00")
            {
                invoice.Status = DAL.Enums.PaymentStatus.Completed;
                invoice.PaidDate = DateTime.Now;
                invoice.TransactionReference = dto.vnp_TransactionNo;
                _unitOfWork.PaymentInvoices.Update(invoice);
            }

            await _unitOfWork.CompleteAsync();

            return MapToDTO(payment);
        }

        public async Task<PaymentDTO> GetPaymentByIdAsync(int id)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(id);
            return payment == null ? null : MapToDTO(payment);
        }

        public async Task<IEnumerable<PaymentDTO>> GetPaymentsByInvoiceAsync(int invoiceId)
        {
            var payments = await _unitOfWork.Payments.FindAsync(x => x.PaymentInvoiceId == invoiceId);
            return payments.Select(MapToDTO);
        }

        public async Task<IEnumerable<PaymentDTO>> GetPaymentsByTenantAsync(int tenantId)
        {
            var payments = await _unitOfWork.Payments.FindAsync(x => x.TenantId == tenantId);
            return payments.Select(MapToDTO);
        }

        public async Task<bool> UpdatePaymentStatusAsync(int id, string status)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(id);
            if (payment == null)
                return false;

            if (Enum.TryParse<DAL.Enums.PaymentStatus>(status, out var parsedStatus))
            {
                payment.Status = parsedStatus;
                _unitOfWork.Payments.Update(payment);
                await _unitOfWork.CompleteAsync();
                return true;
            }

            return false;
        }

        private PaymentDTO MapToDTO(Payment payment)
        {
            return new PaymentDTO
            {
                Id = payment.Id,
                PaymentInvoiceId = payment.PaymentInvoiceId,
                TenantId = payment.TenantId,
                PaymentDate = payment.PaymentDate,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                TransactionId = payment.TransactionId,
                ResponseCode = payment.ResponseCode,
                ResponseMessage = payment.ResponseMessage,
                Status = (int)payment.Status
            };
        }
    }
}
