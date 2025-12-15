using System.Threading.Tasks;

namespace RentalPropertyManagement.BLL.Interfaces
{
    public interface INotificationService
    {
        Task SendContractNotificationAsync(string title, string body, string url);
    }
}