using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace RentalPropertyManagement.Web.Pages.Landlord
{
    // Thêm phân quyền chỉ cho Landlord truy cập
    [Authorize(Roles = "Landlord")]
    public class DashboardModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Kiểm tra user đã đăng nhập chưa
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Login");
            }

            // Kiểm tra Role để đảm bảo đúng người
            if (!User.IsInRole("Landlord"))
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }
    }
}