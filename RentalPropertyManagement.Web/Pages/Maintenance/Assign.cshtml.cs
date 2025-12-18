using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RentalPropertyManagement.BLL.Interfaces;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Web.Pages.Maintenance
{
    public class AssignModel : PageModel
    {
        private readonly IMaintenanceService _maintenanceService;
        private readonly IUserService _userService;

        public AssignModel(IMaintenanceService maintenanceService, IUserService userService)
        {
            _maintenanceService = maintenanceService;
            _userService = userService;
        }

        [BindProperty]
        public int RequestId { get; set; }
        [BindProperty]
        public int SelectedProviderId { get; set; }

        public string PropertyAddress { get; set; }
        public SelectList Providers { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var request = await _maintenanceService.GetRequestByIdAsync(id);
            if (request == null) return NotFound();

            RequestId = id;
            PropertyAddress = request.Property?.Address;

            // Lấy danh sách thợ (ServiceProvider) để chọn
            var providers = await _userService.GetUsersByRoleAsync(DAL.Enums.UserRole.ServiceProvider);
            Providers = new SelectList(providers, "Id", "FullName");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _maintenanceService.AssignProviderAsync(RequestId, SelectedProviderId);
            TempData["Success"] = "Đã phân công thợ thành công!";
            return RedirectToPage("./Inbox");
        }
    }
}