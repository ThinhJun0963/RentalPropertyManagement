using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using RentalPropertyManagement.DAL.Interfaces;
using System.Linq; // Cần dùng Linq để đếm
using System.Security.Claims;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Web.Pages.Tenant
{
    // Đảm bảo chỉ Tenant mới truy cập được
    [Authorize(Roles = "Tenant")]
    public class DashboardModel : PageModel
    {
        private readonly IContractService _contractService;
        private readonly IUnitOfWork _unitOfWork;
        public DashboardModel(IContractService contractService, IUnitOfWork unitOfWork)
        {
            _contractService = contractService;
            _unitOfWork = unitOfWork; // Gán giá trị được inject vào biến private
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
                    var pendingInvoices = await _unitOfWork.Contracts.FindAsync(c => c.TenantId == tenantId && c.Status == DAL.Enums.ContractStatus.Active); // Giả định invoices từ contracts
                    Summary.PendingInvoices = pendingInvoices.Count(); // Thay bằng real nếu có Invoice entity
                    var openRequests = await _unitOfWork.MaintenanceRequests.FindAsync(mr => mr.TenantId == tenantId && mr.Status == DAL.Enums.RequestStatus.New);
                    Summary.OpenMaintenanceRequests = openRequests.Count();
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