using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Web.Pages.Contracts
{
    [Authorize(Roles = "Landlord")]
    public class CreateModel : PageModel
    {
        private readonly IContractService _contractService;
        private readonly IUserService _userService;
        private readonly IPropertyService _propertyService; // Dịch vụ giả định

        public CreateModel(IContractService contractService, IUserService userService, IPropertyService propertyService)
        {
            _contractService = contractService;
            _userService = userService;
            _propertyService = propertyService;
        }

        [BindProperty]
        public ContractDTO Contract { get; set; }

        public SelectList Tenants { get; set; }
        public SelectList Properties { get; set; }

        public async Task OnGetAsync()
        {
            // Lấy danh sách Tenants
            var tenants = await _userService.GetTenantsForSelectionAsync();
            Tenants = new SelectList(tenants, "Id", "FullName");

            // Lấy danh sách Property có sẵn
            var properties = await _propertyService.GetAvailablePropertiesForSelectionAsync();
            Properties = new SelectList(properties, "Id", "Address");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync(); // Load lại dropdown nếu lỗi validation
                return Page();
            }

            try
            {
                await _contractService.CreateContractAsync(Contract);
                TempData["SuccessMessage"] = "Tạo hợp đồng thành công! Hợp đồng đang ở trạng thái PENDING.";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Không thể tạo hợp đồng: " + ex.Message);
                await OnGetAsync();
                return Page();
            }
        }
    }
}