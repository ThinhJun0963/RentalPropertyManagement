using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using RentalPropertyManagement.DAL.Interfaces;
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
        private readonly IUnitOfWork _unitOfWork;

        public EditModel(IContractService contractService, IUnitOfWork unitOfWork)
        {
            _contractService = contractService;
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public ContractDTO Contract { get; set; }

        public IEnumerable<SelectListItem> Tenants { get; set; }
        public IEnumerable<SelectListItem> Properties { get; set; }

        // Phương thức hỗ trợ tải dữ liệu cho Dropdown
        private void LoadSelectLists()
        {
            // Tải danh sách Người thuê
            var users = _unitOfWork.Users.Find(u => u.Role == UserRole.Tenant);
            Tenants = users.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = $"{u.FirstName} {u.LastName} ({u.Email})"
            }).ToList();

            // Tải danh sách Tài sản
            var properties = _unitOfWork.Properties.GetAll();
            Properties = properties.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Address
            }).ToList();
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Contract = await _contractService.GetContractByIdAsync(id);

            if (Contract == null)
            {
                return NotFound();
            }

            LoadSelectLists();
            return Page();
        }

        // --- CẬP NHẬT (UPDATE) ---
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                LoadSelectLists();
                return Page();
            }

            try
            {
                await _contractService.UpdateContractAsync(Contract);
            }
            catch (Exception)
            {
                // Xử lý lỗi khi cập nhật (ví dụ: NotFound, Database concurrency)
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi cập nhật hợp đồng.");
                LoadSelectLists();
                return Page();
            }

            TempData["SuccessMessage"] = $"Hợp đồng ID {Contract.Id} đã được cập nhật thành công.";
            return RedirectToPage("./Index");
        }

        // --- XÓA (DELETE) ---
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _contractService.DeleteContractAsync(id);

            TempData["SuccessMessage"] = $"Hợp đồng ID {id} đã được xóa thành công.";
            return RedirectToPage("./Index");
        }
    }
}