using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using RentalPropertyManagement.DAL.Interfaces;
using RentalPropertyManagement.DAL.Models; // Nhớ using Model
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
            var contracts = _unitOfWork.Contracts.GetAll();

            // Map Entity -> DTO
            var dtos = contracts.Select(c => new ContractDTO
            {
                Id = c.Id,
                TenantName = c.TenantName,
                PropertyAddress = c.PropertyAddress,
                MonthlyRent = c.MonthlyRent,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                IsActive = c.IsActive
            });

            return await Task.FromResult(dtos);
        }

        public async Task<ContractDTO> GetContractByIdAsync(int id)
        {
            var contract = _unitOfWork.Contracts.Get(id);
            if (contract == null) return null;

            return await Task.FromResult(new ContractDTO
            {
                Id = contract.Id,
                TenantName = contract.TenantName,
                PropertyAddress = contract.PropertyAddress,
                MonthlyRent = contract.MonthlyRent,
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                IsActive = contract.IsActive
            });
        }

        public async Task<ContractDTO> CreateContractAsync(ContractDTO dto)
        {
            // Map DTO -> Entity
            var contract = new Contract
            {
                TenantName = dto.TenantName,
                PropertyAddress = dto.PropertyAddress,
                MonthlyRent = dto.MonthlyRent,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = true // Mặc định khi tạo mới
            };

            _unitOfWork.Contracts.Add(contract);
            _unitOfWork.Save(); // Lưu xuống DB để có ID

            dto.Id = contract.Id; // Cập nhật ID trả về
            return await Task.FromResult(dto);
        }

        public async Task UpdateContractAsync(int id, ContractDTO dto)
        {
            var existingContract = _unitOfWork.Contracts.Get(id);
            if (existingContract != null)
            {
                // Cập nhật thông tin
                existingContract.TenantName = dto.TenantName;
                existingContract.PropertyAddress = dto.PropertyAddress;
                existingContract.MonthlyRent = dto.MonthlyRent;
                existingContract.StartDate = dto.StartDate;
                existingContract.EndDate = dto.EndDate;
                existingContract.IsActive = dto.IsActive;

                _unitOfWork.Save();
            }
            await Task.CompletedTask;
        }

        public async Task DeleteContractAsync(int id)
        {
            var contract = _unitOfWork.Contracts.Get(id);
            if (contract != null)
            {
                _unitOfWork.Contracts.Remove(contract);
                _unitOfWork.Save();
            }
            await Task.CompletedTask;
        }
    }
}