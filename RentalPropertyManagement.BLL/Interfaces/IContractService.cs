using RentalPropertyManagement.BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RentalPropertyManagement.BLL.Interfaces
{
    public interface IContractService
    {
        // Các phương thức mẫu - team sẽ mở rộng sau
        Task<IEnumerable<ContractDTO>> GetAllContractsAsync();
        Task<ContractDTO> GetContractByIdAsync(int id);
        Task<ContractDTO> CreateContractAsync(ContractDTO contractDto);
        Task UpdateContractAsync(int id, ContractDTO contractDto);
        Task DeleteContractAsync(int id);

        // Sau này thêm các phương thức nghiệp vụ phức tạp hơn
        // ví dụ: CalculateTotalRent(), GetActiveContracts(), etc.
    }
}