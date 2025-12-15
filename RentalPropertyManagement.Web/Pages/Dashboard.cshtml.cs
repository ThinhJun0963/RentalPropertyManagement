using Microsoft.AspNetCore.Mvc.RazorPages;
using RentalPropertyManagement.BLL.Interfaces;

namespace RentalPropertyManagement.Web.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly IContractService _contractService;

        public DashboardModel(IContractService contractService)
        {
            _contractService = contractService;
        }

        public string Message { get; set; } = "Dashboard is working! DI from Web → BLL is successful.";

        public void OnGet()
        {
            // Chỉ để test DI, chưa gọi method thật vì chưa implement
            // Sau này sẽ gọi _contractService.GetAllContractsAsync() để hiển thị stats
        }
    }
}