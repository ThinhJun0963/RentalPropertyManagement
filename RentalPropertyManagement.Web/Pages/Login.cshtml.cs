using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace RentalPropertyManagement.Web.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IUserService _userService;

        public LoginModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public LoginDto LoginRequest { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public void OnGet(string? returnUrl = null)
        {
            // Hiển thị thông báo thành công nếu vừa đăng ký xong
            if (TempData.ContainsKey("SuccessMessage"))
            {
                ViewData["Success"] = TempData["SuccessMessage"];
            }
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // 1. Gọi Service xác thực (Sử dụng LoginDto đã đồng bộ)
            var user = await _userService.LoginAsync(LoginRequest);

            if (user == null)
            {
                ErrorMessage = "Email hoặc mật khẩu không chính xác.";
                return Page();
            }

            // 2. Thiết lập các thông tin định danh (Claims)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()) // Lưu Role dưới dạng string
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Ghi nhớ đăng nhập
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7) // Hết hạn sau 7 ngày
            };

            // 3. Đăng nhập vào hệ thống
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // 4. Chuyển hướng theo Role hoặc theo ReturnUrl
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Điều hướng về Dashboard tương ứng dựa trên Enum UserRole
            return user.Role.ToString() switch
            {
                "Landlord" => RedirectToPage("/Landlord/Dashboard"),
                "Tenant" => RedirectToPage("/Tenant/Dashboard"),
                "ServiceProvider" => RedirectToPage("/Provider/Dashboard"),
                _ => RedirectToPage("/Index")
            };
        }
    }
}