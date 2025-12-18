using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using System.Security.Claims;

namespace RentalPropertyManagement.Web.Pages.Tenant
{
    public class SubmitRequestModel : PageModel
    {
        private readonly IMaintenanceService _maintenanceService;
        private readonly IContractService _contractService;

        public SubmitRequestModel(IMaintenanceService maintenanceService, IContractService contractService)
        {
            _maintenanceService = maintenanceService;
            _contractService = contractService;
        }

        [BindProperty]
        public MaintenanceRequestDTO RequestInput { get; set; } = new();

        public SelectList MyProperties { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return RedirectToPage("/Login");

            int tenantId = int.Parse(userIdClaim);

            var myContracts = await _contractService.GetContractsByTenantIdAsync(tenantId);
            var activeProperties = myContracts
                .Where(c => c.Status == DAL.Enums.ContractStatus.Active)
                .Select(c => new { c.PropertyId, c.PropertyAddress });

            MyProperties = new SelectList(activeProperties, "PropertyId", "PropertyAddress");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Load lại danh sách nếu validate thất bại
                await OnGetAsync();
                return Page();
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            RequestInput.TenantId = int.Parse(userIdClaim!);

            await _maintenanceService.CreateRequestAsync(RequestInput);

            TempData["SuccessMessage"] = "Yêu cầu bảo trì đã được gửi!";
            return RedirectToPage("/Tenant/Dashboard");
        }
    }
}