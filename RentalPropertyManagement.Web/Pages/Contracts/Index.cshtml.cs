using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Web.Pages.Contracts
{
    // Chỉ Landlord mới được xem danh sách đầy đủ
    [Authorize(Roles = "Landlord")]
    public class IndexModel : PageModel
    {
        private readonly IContractService _contractService;

        public IndexModel(IContractService contractService)
        {
            _contractService = contractService;
        }

        public IEnumerable<ContractDTO> Contracts { get; set; }

        public async Task OnGetAsync()
        {
            // Lấy tất cả hợp đồng (bao gồm Pending, Active, Expired)
            Contracts = await _contractService.GetAllContractsAsync();
        }
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                await _contractService.DeleteContractAsync(id);
                TempData["SuccessMessage"] = $"Hợp đồng ID {id} đã được xóa.";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi xóa: {ex.Message}";
                return RedirectToPage("./Index");
            }
        }
    }
}