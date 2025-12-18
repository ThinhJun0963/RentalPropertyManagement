using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;

namespace RentalPropertyManagement.Web.Pages.Tenant
{
    [Authorize]
    public class PayRentModel : PageModel
    {
        private readonly IPaymentInvoiceService _paymentInvoiceService;

        public IEnumerable<PaymentInvoiceDTO> Invoices { get; set; } = new List<PaymentInvoiceDTO>();

        public PayRentModel(IPaymentInvoiceService paymentInvoiceService)
        {
            _paymentInvoiceService = paymentInvoiceService;
        }

        public async Task OnGetAsync()
        {
            var tenantIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (int.TryParse(tenantIdClaim, out var tenantId))
            {
                Invoices = await _paymentInvoiceService.GetInvoicesByTenantAsync(tenantId);
            }
        }
    }
}
