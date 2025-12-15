using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering; // Cần có
using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces; // Cần có IUserService và IPropertyService
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Web.Pages.Contracts
{
    [Authorize(Roles = "Landlord")]
    public class EditModel : PageModel
    {
        private readonly IContractService _contractService;
        // ⚠️ THAY THẾ IUnitOfWork bằng các service BLL
        private readonly IUserService _userService;
        private readonly IPropertyService _propertyService;

        // ⚠️ Cập nhật Constructor
        public EditModel(IContractService contractService, IUserService userService, IPropertyService propertyService)
        {
            _contractService = contractService;
            _userService = userService;
            _propertyService = propertyService;
        }

        [BindProperty]
        public ContractDTO Contract { get; set; }

        public SelectList Tenants { get; set; } // Thay đổi từ IEnumerable<SelectListItem> sang SelectList
        public SelectList Properties { get; set; } // Thay đổi từ IEnumerable<SelectListItem> sang SelectList

        // ⚠️ Cập nhật Phương thức hỗ trợ tải dữ liệu (Sử dụng BLL Services)
        private async Task LoadSelectListsAsync()
        {
            // Tải danh sách Người thuê (Dùng IUserService)
            var tenants = await _userService.GetTenantsForSelectionAsync();
            Tenants = new SelectList(tenants, "Id", "FullName");

            // Tải danh sách Tài sản (Dùng IPropertyService)
            var properties = await _propertyService.GetAvailablePropertiesForSelectionAsync();
            Properties = new SelectList(properties, "Id", "Address");

            // Nếu hợp đồng đang chỉnh sửa có PropertyId và TenantId, đảm bảo chúng được chọn
            // Logic này phức tạp hơn nếu Property đang active, nhưng để đơn giản, ta chỉ cần có chúng trong danh sách.
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Contract = await _contractService.GetContractByIdAsync(id);

            if (Contract == null)
            {
                return NotFound();
            }

            await LoadSelectListsAsync();
            return Page();
        }

        // --- CẬP NHẬT (UPDATE) ---
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadSelectListsAsync();
                return Page();
            }

            try
            {
                // Gọi BLL Service để Update
                await _contractService.UpdateContractAsync(Contract);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi cập nhật hợp đồng: " + ex.Message);
                await LoadSelectListsAsync();
                return Page();
            }

            TempData["SuccessMessage"] = $"Hợp đồng ID {Contract.Id} đã được cập nhật thành công.";
            return RedirectToPage("./Index");
        }

        // --- XÓA (DELETE) ---
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                await _contractService.DeleteContractAsync(id);
                TempData["SuccessMessage"] = $"Hợp đồng ID {id} đã được xóa thành công.";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi xóa hợp đồng ID {id}: {ex.Message}";
                return RedirectToPage("./Edit", new { id = id });
            }
        }
    }
}