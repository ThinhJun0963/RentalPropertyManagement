using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using RentalPropertyManagement.DAL.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RentalPropertyManagement.BLL.Services
{
    public class ContractService : IContractService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContractService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<IEnumerable<ContractDTO>> GetAllContractsAsync()
        {
            // TODO: Implement sau khi có Model và Repository cụ thể
            throw new System.NotImplementedException();
        }

        public Task<ContractDTO> GetContractByIdAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<ContractDTO> CreateContractAsync(ContractDTO contractDto)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateContractAsync(int id, ContractDTO contractDto)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteContractAsync(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}