using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using RentalPropertyManagement.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Web.Pages.Contracts
{
    [Authorize(Roles = "Landlord")]
    public class CreateModel : PageModel
    {
        private readonly IContractService _contractService;

        // Bạn cần thêm IUserService và IPropertyService để lấy danh sách Tenant và Property
        // Tạm thời, tôi giả định bạn sẽ có IRepository<User> và IRepository<Property> để làm mẫu
        private readonly IUnitOfWork _unitOfWork;

        public CreateModel(IContractService contractService, IUnitOfWork unitOfWork)
        {
            _contractService = contractService;
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public ContractDTO Contract { get; set; }

        public IEnumerable<SelectListItem> Tenants { get; set; }
        public IEnumerable<SelectListItem> Properties { get; set; }

        public void LoadSelectLists()
        {
            // Tải danh sách Người thuê (Role = Tenant)
            var users = _unitOfWork.Users.Find(u => u.Role == DAL.Enums.UserRole.Tenant);
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

        public IActionResult OnGet()
        {
            LoadSelectLists();
            Contract = new ContractDTO
            {
                StartDate = System.DateTime.Now,
                EndDate = System.DateTime.Now.AddYears(1)
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Kiểm tra tính hợp lệ
            if (!ModelState.IsValid)
            {
                LoadSelectLists();
                return Page();
            }

            // Gọi Service để tạo mới
            var newContract = await _contractService.CreateContractAsync(Contract);

            TempData["SuccessMessage"] = $"Hợp đồng ID {newContract.Id} đã được tạo thành công!";

            return RedirectToPage("./Index");
        }
    }
}