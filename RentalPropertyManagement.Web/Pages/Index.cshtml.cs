using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using RentalPropertyManagement.BLL.Interfaces;
using RentalPropertyManagement.DAL.Enums;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace RentalPropertyManagement.Web.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IContractService _contractService;

        // Thuộc tính string để lưu role đã lấy
        public string UserRole { get; set; }
        public string UserFullName { get; set; }

        public int TotalContracts { get; set; }
        public int ActiveContracts { get; set; }
        public int ExpiringSoonContracts { get; set; }
        public decimal TotalMonthlyRent { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IContractService contractService)
        {
            _logger = logger;
            _contractService = contractService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Login");
            }

            UserRole = User.FindFirstValue(ClaimTypes.Role);
            UserFullName = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(UserRole))
            {
                return RedirectToPage("/AccessDenied");
            }

            // Dùng UserRole (enum type) được định nghĩa trong DAL.Enums để so sánh.
            // Sửa lỗi: Thay vì dùng tên UserRole (string) bị trùng, chúng ta chỉ cần dùng giá trị enum
            if (UserRole == RentalPropertyManagement.DAL.Enums.UserRole.Landlord.ToString())
            {
                var allContracts = await _contractService.GetAllContractsAsync();

                TotalContracts = allContracts.Count();
                ActiveContracts = allContracts.Count(c => c.Status == ContractStatus.Active);
                TotalMonthlyRent = allContracts.Where(c => c.Status == ContractStatus.Active).Sum(c => c.RentAmount);

                var today = DateTime.Today;
                var ninetyDaysFromNow = today.AddDays(90);

                ExpiringSoonContracts = allContracts.Count(c =>
                    c.Status == ContractStatus.Active &&
                    c.EndDate.HasValue &&
                    c.EndDate.Value.Date >= today &&
                    c.EndDate.Value.Date <= ninetyDaysFromNow);
            }

            return Page();
        }
    }
}