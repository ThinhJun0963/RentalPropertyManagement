using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Web.Hubs
{
    // Lớp Hub chính
    public class MainHub : Hub
    {
        // Phương thức nhận thông báo từ Client (nếu cần)
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        // Phương thức ReceiveNotification được gọi từ Server (SignalRNotificationService)
    }
}