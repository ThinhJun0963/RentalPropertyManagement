using RentalPropertyManagement.BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RentalPropertyManagement.BLL.Interfaces
{
    public interface IContractService
    {
        // Lấy toàn bộ danh sách hợp đồng (cho Landlord)
        Task<IEnumerable<ContractDTO>> GetAllContractsAsync();

        // Lấy danh sách hợp đồng của một người thuê cụ thể
        Task<IEnumerable<ContractDTO>> GetContractsByTenantIdAsync(int tenantId);

        // Lấy chi tiết một hợp đồng
        Task<ContractDTO> GetContractByIdAsync(int id);

        // Tạo hợp đồng mới
        Task AddContractAsync(ContractDTO contractDto);

        // Cập nhật thông tin hoặc trạng thái hợp đồng (Active, Expired, Terminated)
        Task UpdateContractAsync(ContractDTO contractDto);

        // Xóa hợp đồng
        Task DeleteContractAsync(int id);
    }
}