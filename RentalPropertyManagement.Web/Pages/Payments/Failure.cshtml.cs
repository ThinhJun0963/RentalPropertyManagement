using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RentalPropertyManagement.Web.Pages.Payments
{
    public class FailureModel : PageModel
    {
        public string ErrorMessage { get; set; }
        public string ResponseCode { get; set; }
        public int InvoiceId { get; set; }

        public void OnGet(string error, string code, int invoiceId)
        {
            ErrorMessage = error;
            ResponseCode = code;
            InvoiceId = invoiceId;
        }
    }
}
