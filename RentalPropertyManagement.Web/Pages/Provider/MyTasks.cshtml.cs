using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using RentalPropertyManagement.DAL.Enums;
using System.Security.Claims;

namespace RentalPropertyManagement.Web.Pages.Provider
{
    public class MyTasksModel : PageModel
    {
        private readonly IMaintenanceService _maintenanceService;

        public MyTasksModel(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        public IEnumerable<MaintenanceRequestDTO> Tasks { get; set; } = new List<MaintenanceRequestDTO>();

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return RedirectToPage("/Login");

            int providerId = int.Parse(userIdClaim);
            Tasks = await _maintenanceService.GetTasksByProviderIdAsync(providerId);

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(int id, RequestStatus status)
        {
            await _maintenanceService.UpdateStatusAsync(id, status);
            TempData["Success"] = $"Yêu cầu #{id} đã cập nhật thành {status}.";
            return RedirectToPage();
        }
    }
}