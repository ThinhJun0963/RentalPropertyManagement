using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace RentalPropertyManagement.Web.Pages.Landlord
{
    public class DashboardModel : PageModel
    {
        public void OnGet()
        {
            // Kiểm tra user đã đăng nhập
            if (User?.FindFirst(ClaimTypes.NameIdentifier) == null)
            {
                RedirectToPage("/Login");
            }
        }
    }
}
