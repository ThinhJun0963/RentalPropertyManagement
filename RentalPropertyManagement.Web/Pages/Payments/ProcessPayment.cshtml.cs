using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;

namespace RentalPropertyManagement.Web.Pages.Payments
{
    [Authorize]
    public class ProcessPaymentModel : PageModel
    {
        private readonly IPaymentInvoiceService _paymentInvoiceService;
        private readonly IPaymentService _paymentService;

        [BindProperty]
        public PaymentInvoiceDTO Invoice { get; set; }

        public ProcessPaymentModel(IPaymentInvoiceService paymentInvoiceService, IPaymentService paymentService)
        {
            _paymentInvoiceService = paymentInvoiceService;
            _paymentService = paymentService;
        }

        public async Task<IActionResult> OnGetAsync(int invoiceId)
        {
            Invoice = await _paymentInvoiceService.GetInvoiceByIdAsync(invoiceId);
            if (Invoice == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var invoiceId = Invoice?.Id ?? 0;
            if (invoiceId <= 0)
            {
                return NotFound();
            }

            Invoice = await _paymentInvoiceService.GetInvoiceByIdAsync(invoiceId);
            if (Invoice == null)
            {
                return NotFound();
            }

            try
            {
                var ipAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
                
                var dto = new VNPayPaymentDTO
                {
                    PaymentInvoiceId = invoiceId,
                    ReturnUrl = $"{Request.Scheme}://{Request.Host}/Payments/Callback",
                    IpAddress = ipAddress
                };

                var paymentUrl = await _paymentService.CreateVNPayPaymentUrlAsync(dto);
                return Redirect(paymentUrl);
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", $"Payment processing failed: {ex.Message}");
                return Page();
            }
        }
    }
}
