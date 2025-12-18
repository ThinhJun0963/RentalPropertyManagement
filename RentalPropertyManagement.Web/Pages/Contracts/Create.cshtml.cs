using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using RentalPropertyManagement.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Web.Pages.Contracts
{
    [Authorize(Roles = "Landlord")]
    public class CreateModel : PageModel
    {
        private readonly IContractService _contractService;
        private readonly IPropertyService _propertyService;
        private readonly IUserService _userService;

        public CreateModel(IContractService contractService, IPropertyService propertyService, IUserService userService)
        {
            _contractService = contractService;
            _propertyService = propertyService;
            _userService = userService;
        }

        [BindProperty]
        public ContractDTO Contract { get; set; } = new ContractDTO { StartDate = DateTime.Now };

        public IEnumerable<SelectListItem> PropertyList { get; set; }
        public IEnumerable<SelectListItem> TenantList { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadData();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Kiểm tra ModelState
            if (!ModelState.IsValid)
            {
                // Nếu form không gửi được, nạp lại dữ liệu Dropdown
                await LoadData();
                return Page();
            }

            try
            {
                // Mặc định trạng thái khi tạo mới là Pending
                Contract.Status = ContractStatus.Pending;

                await _contractService.AddContractAsync(Contract);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                // Nếu có lỗi từ Database (ví dụ: lỗi khóa ngoại), hiển thị lỗi lên màn hình
                ModelState.AddModelError(string.Empty, "Lỗi khi lưu vào CSDL: " + ex.Message);
                await LoadData();
                return Page();
            }
        }

        private async Task LoadData()
        {
            // Lấy danh sách tài sản trống
            var availableProperties = await _propertyService.GetAvailablePropertiesForSelectionAsync();
            PropertyList = availableProperties.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Address} - {p.MonthlyRent:N0} VNĐ"
            }).ToList();

            // Lấy danh sách người thuê
            var tenants = await _userService.GetTenantsForSelectionAsync();
            TenantList = tenants.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.FullName
            }).ToList();
        }
    }
}