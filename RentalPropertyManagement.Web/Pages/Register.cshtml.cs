using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using RentalPropertyManagement.DAL.Enums;

namespace RentalPropertyManagement.Web.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly IUserService _userService;

        public RegisterModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public RegisterDto RegisterRequest { get; set; } = new();

        public string? ErrorMessage { get; set; }

        // Danh sách để hiển thị lên Dropdown
        public List<SelectListItem> RoleOptions { get; set; } = new();

        public void OnGet()
        {
            LoadRoles();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                LoadRoles();
                ErrorMessage = "Vui lòng kiểm tra lại thông tin đăng ký.";
                return Page();
            }

            var result = await _userService.RegisterAsync(RegisterRequest);

            if (result)
            {
                TempData["SuccessMessage"] = "Đăng ký thành công! Bạn có thể đăng nhập ngay.";
                return RedirectToPage("/Login");
            }
            else
            {
                LoadRoles();
                ErrorMessage = "Email đã tồn tại hoặc có lỗi xảy ra.";
                return Page();
            }
        }

        private void LoadRoles()
        {
            RoleOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = ((int)UserRole.Tenant).ToString(), Text = "Người thuê (Tenant)" },
                new SelectListItem { Value = ((int)UserRole.Landlord).ToString(), Text = "Chủ nhà (Landlord)" },
                new SelectListItem { Value = ((int)UserRole.ServiceProvider).ToString(), Text = "Thợ sửa chữa (Provider)" }
            };
        }
    }
}