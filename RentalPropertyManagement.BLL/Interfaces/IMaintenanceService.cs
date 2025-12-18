using RentalPropertyManagement.BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RentalPropertyManagement.BLL.Interfaces
{
    public interface IMaintenanceService
    {
        Task<MaintenanceRequestDTO> CreateRequestAsync(MaintenanceRequestDTO dto);
        Task<IEnumerable<MaintenanceRequestDTO>> GetRequestsByTenantIdAsync(int tenantId);
        Task<IEnumerable<MaintenanceRequestDTO>> GetAllRequestsAsync();
        Task<MaintenanceRequestDTO> GetRequestByIdAsync(int id);
        Task UpdateStatusAsync(int requestId, DAL.Enums.RequestStatus newStatus);
        Task AssignProviderAsync(int requestId, int providerId);

        // Phương thức gây lỗi nếu thiếu:
        Task<IEnumerable<MaintenanceRequestDTO>> GetTasksByProviderIdAsync(int providerId);
    }
}