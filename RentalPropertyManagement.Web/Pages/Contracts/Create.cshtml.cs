using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Web.Pages.Contracts
{
    public class CreateModel : PageModel
    {
        private readonly IContractService _contractService;

        public CreateModel(IContractService contractService)
        {
            _contractService = contractService;
        }

        [BindProperty]
        public ContractDTO Contract { get; set; }

        public void OnGet()
        {
            // Khởi tạo giá trị mặc định nếu cần
            Contract = new ContractDTO { StartDate = System.DateTime.Now };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _contractService.CreateContractAsync(Contract);

            // Sau này bạn sẽ thêm SignalR ở đây để thông báo:
            // await _hubContext.Clients.All.SendAsync("ReceiveMessage", "New contract created!");

            return RedirectToPage("./Index"); // Cần tạo trang Index để list ra
        }
    }
}