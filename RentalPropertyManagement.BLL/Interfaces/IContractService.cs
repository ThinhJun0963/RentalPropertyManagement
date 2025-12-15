using RentalPropertyManagement.BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RentalPropertyManagement.BLL.Interfaces
{
    public interface IContractService
    {
        Task<IEnumerable<ContractDTO>> GetAllContractsAsync();
        Task<ContractDTO> GetContractByIdAsync(int id);
        Task<ContractDTO> CreateContractAsync(ContractDTO contractDto);
        Task UpdateContractAsync(ContractDTO contractDto);
        Task DeleteContractAsync(int id);
    }
}