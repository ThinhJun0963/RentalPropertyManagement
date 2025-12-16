using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;

namespace RentalPropertyManagement.Web.Hubs
{
    public class MainHub : Hub
    {
        // Gửi tin nhắn đến tất cả mọi người (Global Chat)
        public async Task SendGlobalMessage(string message)
        {
            // Lấy tên người dùng từ Cookie đăng nhập (nếu chưa đăng nhập thì là "Khách")
            var user = Context.User?.Identity?.Name ?? "Khách";

            // Lấy thời gian hiện tại
            var timestamp = DateTime.Now.ToString("HH:mm");

            // Gửi về Client: User, Message, Time
            await Clients.All.SendAsync("ReceiveGlobalMessage", user, message, timestamp);
        }
    }
}