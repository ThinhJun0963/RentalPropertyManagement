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

        // --- Helper Mapping (Chuyển Entity -> DTO) ---
        private ContractDTO MapToDto(Contract entity)
        {
            if (entity == null) return null;
            return new ContractDTO
            {
                Id = entity.Id,
                PropertyId = entity.PropertyId,
                TenantId = entity.TenantId,
                RentAmount = entity.RentAmount,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Status = entity.Status,
                // TODO: Sau này cần JOIN/Include User và Property để lấy tên/địa chỉ
                TenantName = entity.Tenant != null ? $"{entity.Tenant.FirstName} {entity.Tenant.LastName}" : "N/A",
                PropertyAddress = entity.Property != null ? entity.Property.Address : "N/A"
            };
        }
        // ----------------------------------------------

        public async Task<IEnumerable<ContractDTO>> GetAllContractsAsync()
        {
            // Lấy tất cả hợp đồng từ DAL, bao gồm cả Tenant và Property để hiển thị tên và địa chỉ
            var contracts = await _unitOfWork.Contracts
                .GetAllAsync(); // Bạn đang dùng IRepository<Contract>, chúng ta sẽ phải xem lại sau

            // Tạm thời, vì IRepository chung của bạn chưa có Include, chúng ta dùng Find/GetAll với query
            // Nhưng hiện tại IRepository của bạn chưa có phương thức Async với Include, nên tôi sẽ dùng phương thức đơn giản nhất.

            var entities = await Task.Run(() => _unitOfWork.Contracts.GetAll().ToList()); // Giả lập Async

            // Ánh xạ và trả về
            return entities.Select(MapToDto);
        }

        public async Task<ContractDTO> GetContractByIdAsync(int id)
        {
            var contract = await _unitOfWork.Contracts.GetAsync(id);
            return MapToDto(contract);
        }

        public async Task<ContractDTO> CreateContractAsync(ContractDTO dto)
        {
            // Map DTO -> Entity
            var contract = new Contract
            {
                PropertyId = dto.PropertyId,
                TenantId = dto.TenantId,
                RentAmount = dto.RentAmount,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status
            };

            await _unitOfWork.Contracts.AddAsync(contract);
            await _unitOfWork.CompleteAsync();

            dto.Id = contract.Id; // Lấy ID mới tạo
            return dto;
        }

        public async Task UpdateContractAsync(ContractDTO dto)
        {
            var existingContract = await _unitOfWork.Contracts.GetAsync(dto.Id);

            if (existingContract != null)
            {
                // Cập nhật các thuộc tính
                existingContract.PropertyId = dto.PropertyId;
                existingContract.TenantId = dto.TenantId;
                existingContract.RentAmount = dto.RentAmount;
                existingContract.StartDate = dto.StartDate;
                existingContract.EndDate = dto.EndDate;
                existingContract.Status = dto.Status;

                // Lưu thay đổi. (Repository<T> Add/Remove không cần, chỉ cần cho Update)
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeleteContractAsync(int id)
        {
            var contract = await _unitOfWork.Contracts.GetAsync(id);
            if (contract != null)
            {
                // Remove thường là đồng bộ trong EF Core (trừ khi dùng RemoveRangeAsync)
                _unitOfWork.Contracts.Remove(contract);
                await _unitOfWork.CompleteAsync();
            }
        }
    }
}