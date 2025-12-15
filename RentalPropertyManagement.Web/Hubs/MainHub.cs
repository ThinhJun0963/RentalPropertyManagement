using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Web.Hubs
{
    // Kế thừa từ Hub để có quyền truy cập vào Clients
    public class MainHub : Hub
    {
        // Hàm này được Client gọi
        public async Task SendNotification(string title, string body, string url = "")
        {
            // Giả sử gửi đến tất cả Landlord và Tenant
            await Clients.All.SendAsync("ReceiveNotification", title, body, url);
        }
    }
}