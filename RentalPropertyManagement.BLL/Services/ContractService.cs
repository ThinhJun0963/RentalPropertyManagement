using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using RentalPropertyManagement.DAL.Entities;
using RentalPropertyManagement.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<ContractDTO>> GetAllContractsAsync()
        {
            var contracts = await _unitOfWork.Contracts.GetAllAsync(c => c.Property, c => c.Tenant);
            return contracts.Select(c => MapToDTO(c));
        }

        public async Task<IEnumerable<ContractDTO>> GetContractsByTenantIdAsync(int tenantId)
        {
            var contracts = await _unitOfWork.Contracts.FindAsync(c => c.TenantId == tenantId, c => c.Property, c => c.Tenant);
            return contracts.Select(c => MapToDTO(c));
        }


        public async Task<ContractDTO> GetContractByIdAsync(int id)
        {

            var contract = await _unitOfWork.Contracts.GetSingleAsync(
                c => c.Id == id,
                c => c.Property,
                c => c.Tenant
            );

            return contract != null ? MapToDTO(contract) : null;
        }

        public async Task AddContractAsync(ContractDTO contractDto)
        {
            var contract = new Contract
            {
                PropertyId = contractDto.PropertyId,
                TenantId = contractDto.TenantId,
                StartDate = contractDto.StartDate,
                EndDate = contractDto.EndDate,
                RentAmount = contractDto.RentAmount, // Đã sửa từ Price thành RentAmount
                Status = contractDto.Status
            };

            await _unitOfWork.Contracts.AddAsync(contract);

            if (contract.Status == DAL.Enums.ContractStatus.Active)
            {
                var property = await _unitOfWork.Properties.GetByIdAsync(contract.PropertyId);
                if (property != null) property.IsOccupied = true;
            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateContractAsync(ContractDTO contractDto)
        {
            var contract = await _unitOfWork.Contracts.GetByIdAsync(contractDto.Id);
            if (contract != null)
            {
                contract.StartDate = contractDto.StartDate;
                contract.EndDate = contractDto.EndDate;
                contract.RentAmount = contractDto.RentAmount; // Đã sửa từ Price thành RentAmount
                contract.Status = contractDto.Status;

                var property = await _unitOfWork.Properties.GetByIdAsync(contract.PropertyId);
                if (property != null)
                {
                    property.IsOccupied = (contract.Status == DAL.Enums.ContractStatus.Active);
                }

                _unitOfWork.Contracts.Update(contract);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeleteContractAsync(int id)
        {
            var contract = await _unitOfWork.Contracts.GetByIdAsync(id);
            if (contract != null)
            {
                _unitOfWork.Contracts.Remove(contract);
                await _unitOfWork.CompleteAsync();
            }
        }

        private ContractDTO MapToDTO(Contract c)
        {
            return new ContractDTO
            {
                Id = c.Id,
                PropertyId = c.PropertyId,
                PropertyAddress = c.Property?.Address ?? "N/A",
                TenantId = c.TenantId,
                // Đã sửa từ FullName thành kết hợp FirstName và LastName
                TenantName = c.Tenant != null ? $"{c.Tenant.FirstName} {c.Tenant.LastName}" : "N/A",
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                RentAmount = c.RentAmount, // Đã sửa từ Price thành RentAmount
                Status = c.Status
            };
        }
    }
}