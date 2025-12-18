using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RentalPropertyManagement.BLL.Interfaces;
using RentalPropertyManagement.DAL.Enums;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace RentalPropertyManagement.Web.Pages.Landlord
{
    [Authorize(Roles = "Landlord")]
    public class DashboardModel : PageModel
    {
        private readonly IContractService _contractService;

        public DashboardModel(IContractService contractService)
        {
            _contractService = contractService;
        }

        public LandlordDashboardSummary Summary { get; set; } = new LandlordDashboardSummary();

        public async Task OnGetAsync()
        {
            var allContracts = await _contractService.GetAllContractsAsync();
            Summary.TotalContracts = allContracts.Count();
            Summary.ActiveContracts = allContracts.Count(c => c.Status == ContractStatus.Active);
            Summary.TotalMonthlyRent = allContracts.Where(c => c.Status == ContractStatus.Active).Sum(c => c.RentAmount);
            var today = DateTime.Today;
            var ninetyDaysFromNow = today.AddDays(90);
            Summary.ExpiringSoonContracts = allContracts.Count(c =>
                c.Status == ContractStatus.Active &&
                c.EndDate.HasValue &&
                c.EndDate.Value.Date >= today &&
                c.EndDate.Value.Date <= ninetyDaysFromNow);
        }
    }

    public class LandlordDashboardSummary
    {
        public int TotalContracts { get; set; }
        public int ActiveContracts { get; set; }
        public decimal TotalMonthlyRent { get; set; }
        public int ExpiringSoonContracts { get; set; }
    }
}