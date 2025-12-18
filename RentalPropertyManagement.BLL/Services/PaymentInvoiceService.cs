using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using RentalPropertyManagement.DAL.Entities;
using RentalPropertyManagement.DAL.Interfaces;

namespace RentalPropertyManagement.BLL.Services
{
    public class PaymentInvoiceService : IPaymentInvoiceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentInvoiceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaymentInvoiceDTO> CreateInvoiceAsync(CreatePaymentInvoiceDTO dto)
        {
            var paymentInvoice = new PaymentInvoice
            {
                ContractId = dto.ContractId,
                TenantId = dto.TenantId,
                InvoiceDate = DateTime.Now,
                DueDate = dto.DueDate,
                Amount = dto.Amount,
                Description = dto.Description,
                Status = DAL.Enums.PaymentStatus.Pending,
                PaidDate = null
            };

            _unitOfWork.PaymentInvoices.Add(paymentInvoice);
            await _unitOfWork.CompleteAsync();

            return MapToDTO(paymentInvoice);
        }

        public async Task<PaymentInvoiceDTO> GetInvoiceByIdAsync(int id)
        {
            var invoice = await _unitOfWork.PaymentInvoices.GetByIdAsync(id);
            return invoice == null ? null : MapToDTO(invoice);
        }

        public async Task<IEnumerable<PaymentInvoiceDTO>> GetInvoicesByContractAsync(int contractId)
        {
            var invoices = await _unitOfWork.PaymentInvoices.FindAsync(x => x.ContractId == contractId);
            return invoices.Select(MapToDTO);
        }

        public async Task<IEnumerable<PaymentInvoiceDTO>> GetInvoicesByTenantAsync(int tenantId)
        {
            var invoices = await _unitOfWork.PaymentInvoices.FindAsync(x => x.TenantId == tenantId);
            return invoices.Select(MapToDTO);
        }

        public async Task<bool> UpdateInvoiceStatusAsync(int id, string status)
        {
            var invoice = await _unitOfWork.PaymentInvoices.GetByIdAsync(id);
            if (invoice == null)
                return false;

            if (Enum.TryParse<DAL.Enums.PaymentStatus>(status, out var parsedStatus))
            {
                invoice.Status = parsedStatus;
                if (parsedStatus == DAL.Enums.PaymentStatus.Completed)
                    invoice.PaidDate = DateTime.Now;

                _unitOfWork.PaymentInvoices.Update(invoice);
                await _unitOfWork.CompleteAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteInvoiceAsync(int id)
        {
            var invoice = await _unitOfWork.PaymentInvoices.GetByIdAsync(id);
            if (invoice == null)
                return false;

            _unitOfWork.PaymentInvoices.Remove(invoice);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        private PaymentInvoiceDTO MapToDTO(PaymentInvoice invoice)
        {
            return new PaymentInvoiceDTO
            {
                Id = invoice.Id,
                ContractId = invoice.ContractId,
                TenantId = invoice.TenantId,
                InvoiceDate = invoice.InvoiceDate,
                DueDate = invoice.DueDate,
                PaidDate = invoice.PaidDate,
                Amount = invoice.Amount,
                Description = invoice.Description,
                Status = (int)invoice.Status,
                TransactionReference = invoice.TransactionReference
            };
        }
    }
}
