using Microsoft.AspNetCore.SignalR;
using RentalPropertyManagement.BLL.Interfaces;
using RentalPropertyManagement.Web.Hubs;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Web.Services
{
    public class SignalRNotificationService : INotificationService
    {
        private readonly IHubContext<MainHub> _hubContext;

        public SignalRNotificationService(IHubContext<MainHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendContractNotificationAsync(string title, string body, string url)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", title, body, url);
        }

        // THÊM MỚI: Triển khai gửi thông báo bảo trì qua SignalR
        public async Task SendMaintenanceNotificationAsync(string title, string body, string url)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMaintenanceUpdate", title, body, url);
        }
    }
}