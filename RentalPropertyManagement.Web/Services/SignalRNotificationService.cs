using Microsoft.AspNetCore.SignalR;
using RentalPropertyManagement.BLL.Interfaces;
using RentalPropertyManagement.Web.Hubs;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Web.Services
{
    // Lớp này thực thi Interface BLL bằng cách sử dụng IHubContext
    public class SignalRNotificationService : INotificationService
    {
        private readonly IHubContext<MainHub> _hubContext;

        public SignalRNotificationService(IHubContext<MainHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendContractNotificationAsync(string title, string body, string url)
        {
            // Gửi tin nhắn đến tất cả clients đang kết nối
            // "ReceiveNotification" là tên hàm Javascript Client sẽ lắng nghe
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", title, body, url);
        }
    }
}