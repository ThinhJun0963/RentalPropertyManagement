using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq; // Cần dùng Linq để đếm

namespace RentalPropertyManagement.Web.Pages.Tenant
{
    // Đảm bảo chỉ Tenant mới truy cập được
    [Authorize(Roles = "Tenant")]
    public class DashboardModel : PageModel
    {
        private readonly IContractService _contractService;

        // Thường bạn sẽ có IInvoiceService và IRequestService ở đây
        // private readonly IInvoiceService _invoiceService;

        public DashboardModel(IContractService contractService)
        {
            _contractService = contractService;
        }

        public TenantDashboardSummary Summary { get; set; } = new TenantDashboardSummary();

        public async Task OnGetAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdClaim, out int tenantId))
                {
                    // Lấy tất cả hợp đồng của tenant
                    var contracts = await _contractService.GetContractsByTenantIdAsync(tenantId);

                    // Tính toán các chỉ số
                    Summary.TotalContracts = contracts.Count();
                    Summary.ActiveContracts = contracts.Count(c => c.Status == DAL.Enums.ContractStatus.Active);

                    // Giả định: 
                    // Summary.PendingInvoices = await _invoiceService.GetPendingInvoiceCountAsync(tenantId);
                    // Summary.OpenMaintenanceRequests = await _requestService.GetOpenRequestCountAsync(tenantId);

                    // Để đơn giản, tôi gán giá trị giả định cho các phần chưa có service
                    Summary.PendingInvoices = 2;
                    Summary.OpenMaintenanceRequests = 1;
                }
            }
        }
    }

    // DTO đơn giản chứa dữ liệu tổng quan cho Dashboard
    public class TenantDashboardSummary
    {
        public int TotalContracts { get; set; }
        public int ActiveContracts { get; set; }
        public int PendingInvoices { get; set; }
        public int OpenMaintenanceRequests { get; set; }
    }
}