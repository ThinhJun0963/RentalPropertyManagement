using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RentalPropertyManagement.Web.Pages.Payments
{
    public class SuccessModel : PageModel
    {
        public string TransactionId { get; set; }
        public int InvoiceId { get; set; }

        public void OnGet(string transactionId, int invoiceId)
        {
            TransactionId = transactionId;
            InvoiceId = invoiceId;
        }
    }
}
