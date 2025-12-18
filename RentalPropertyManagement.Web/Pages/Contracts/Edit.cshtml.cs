using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using RentalPropertyManagement.DAL.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Web.Pages.Contracts
{
    [Authorize(Roles = "Landlord")]
    public class EditModel : PageModel
    {
        private readonly IContractService _contractService;
        private readonly IPropertyService _propertyService;
        private readonly IUserService _userService;

        public EditModel(IContractService contractService, IPropertyService propertyService, IUserService userService)
        {
            _contractService = contractService;
            _propertyService = propertyService;
            _userService = userService;
        }

        [BindProperty]
        public ContractDTO Contract { get; set; }

        public IEnumerable<SelectListItem> PropertyList { get; set; }
        public IEnumerable<SelectListItem> TenantList { get; set; }
        public IEnumerable<SelectListItem> StatusList { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Contract = await _contractService.GetContractByIdAsync(id);

            if (Contract == null)
            {
                return NotFound();
            }

            await LoadData();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadData();
                return Page();
            }

            await _contractService.UpdateContractAsync(Contract);
            return RedirectToPage("./Index");
        }

        private async Task LoadData()
        {
            // Lấy danh sách tài sản: Gồm các tài sản trống + tài sản hiện tại của hợp đồng này
            var allAvailable = await _propertyService.GetAvailablePropertiesForSelectionAsync();
            var currentProperty = await _propertyService.GetPropertyByIdAsync(Contract.PropertyId);

            var propertyOptions = allAvailable.ToList();
            if (currentProperty != null && !propertyOptions.Any(p => p.Id == currentProperty.Id))
            {
                propertyOptions.Add(currentProperty);
            }

            PropertyList = propertyOptions.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Address} ({p.City})"
            });

            // Lấy danh sách người thuê
            var tenants = await _userService.GetTenantsForSelectionAsync();
            TenantList = tenants.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.FullName
            });

            // Danh sách trạng thái hợp đồng từ Enum
            StatusList = System.Enum.GetValues(typeof(ContractStatus))
                .Cast<ContractStatus>()
                .Select(s => new SelectListItem
                {
                    Value = ((int)s).ToString(),
                    Text = s.ToString()
                });
        }
    }
}