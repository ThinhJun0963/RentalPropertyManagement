using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Web.Pages.Contracts
{
    // Chỉ cho phép Chủ nhà truy cập vào danh sách quản lý hợp đồng tổng thể
    [Authorize(Roles = "Landlord")]
    public class IndexModel : PageModel
    {
        private readonly IContractService _contractService;

        public IndexModel(IContractService contractService)
        {
            _contractService = contractService;
        }

        // Danh sách hợp đồng để hiển thị lên bảng
        public IEnumerable<ContractDTO> Contracts { get; set; }

        public async Task OnGetAsync()
        {
            // Lấy toàn bộ hợp đồng kèm thông tin tên người thuê và địa chỉ tài sản
            Contracts = await _contractService.GetAllContractsAsync();
        }

        // Xử lý yêu cầu xóa hợp đồng từ giao diện
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _contractService.DeleteContractAsync(id);

            // Tải lại trang để cập nhật danh sách mới nhất
            return RedirectToPage();
        }
    }
}