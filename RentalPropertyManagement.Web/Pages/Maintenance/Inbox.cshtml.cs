using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Web.Pages.Maintenance
{
    public class InboxModel : PageModel
    {
        private readonly IMaintenanceService _maintenanceService;

        public InboxModel(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        public IEnumerable<MaintenanceRequestDTO> Requests { get; set; }

        public async Task OnGetAsync()
        {
            // Lấy toàn bộ yêu cầu bảo trì từ BLL
            Requests = await _maintenanceService.GetAllRequestsAsync();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(int id, DAL.Enums.RequestStatus status)
        {
            await _maintenanceService.UpdateStatusAsync(id, status);
            return RedirectToPage();
        }
    }
}