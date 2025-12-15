using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Web.Pages.Contracts
{
    // Chỉ cho phép Landlord (Chủ nhà/Quản lý) truy cập trang này
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
            // Lấy toàn bộ danh sách Hợp đồng từ BLL
            Contracts = await _contractService.GetAllContractsAsync();
        }
    }
}