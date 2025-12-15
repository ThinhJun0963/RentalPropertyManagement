using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Web.Pages.Tenant
{
    [Authorize(Roles = "Tenant")]
    public class MyContractsModel : PageModel
    {
        private readonly IContractService _contractService;

        public MyContractsModel(IContractService contractService)
        {
            _contractService = contractService;
        }

        public IEnumerable<ContractDTO> Contracts { get; set; }

        public async Task OnGetAsync()
        {
            // Lấy ID của Tenant đang đăng nhập (Giả định ID được lưu trong Claim)
            if (User.Identity.IsAuthenticated)
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdClaim, out int tenantId))
                {
                    Contracts = await _contractService.GetContractsByTenantIdAsync(tenantId);
                    return;
                }
            }
            Contracts = new List<ContractDTO>();
        }
    }
}