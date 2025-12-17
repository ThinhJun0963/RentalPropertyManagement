using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;

namespace RentalPropertyManagement.Web.Pages.Payments
{
    public class CallbackModel : PageModel
    {
        private readonly IPaymentService _paymentService;

        public string ErrorMessage { get; set; }
        public bool IsSuccessful { get; set; }

        public CallbackModel(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Extract VNPay callback parameters
                var queryParams = Request.Query;
                var callbackDto = new VNPayCallbackDTO
                {
                    vnp_Amount = queryParams["vnp_Amount"],
                    vnp_BankCode = queryParams["vnp_BankCode"],
                    vnp_BankTranNo = queryParams["vnp_BankTranNo"],
                    vnp_CardType = queryParams["vnp_CardType"],
                    vnp_OrderInfo = queryParams["vnp_OrderInfo"],
                    vnp_PayDate = queryParams["vnp_PayDate"],
                    vnp_ResponseCode = queryParams["vnp_ResponseCode"],
                    vnp_TmnCode = queryParams["vnp_TmnCode"],
                    vnp_TransactionNo = queryParams["vnp_TransactionNo"],
                    vnp_TransactionStatus = queryParams["vnp_TransactionStatus"],
                    vnp_TxnRef = queryParams["vnp_TxnRef"],
                    vnp_SecureHash = queryParams["vnp_SecureHash"],
                    vnp_SecureHashType = queryParams["vnp_SecureHashType"]
                };

                // Process the payment
                var payment = await _paymentService.ProcessVNPayCallbackAsync(callbackDto);

                if (callbackDto.vnp_ResponseCode == "00")
                {
                    IsSuccessful = true;
                    return RedirectToPage("Success", new { transactionId = payment.TransactionId, invoiceId = payment.PaymentInvoiceId });
                }
                else
                {
                    IsSuccessful = false;
                    ErrorMessage = $"Payment failed with code: {callbackDto.vnp_ResponseCode}";
                    return RedirectToPage("Failure", new { code = callbackDto.vnp_ResponseCode, invoiceId = callbackDto.vnp_TxnRef });
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = ex.Message;
                return RedirectToPage("Failure", new { error = ex.Message });
            }
        }
    }
}
