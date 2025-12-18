using Microsoft.AspNetCore.SignalR;
using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using RentalPropertyManagement.DAL.Entities;
using RentalPropertyManagement.DAL.Enums;
using RentalPropertyManagement.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.BLL.Services
{
    public class ContractService : IContractService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        public ContractService(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
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
            var entities = await _unitOfWork.Contracts
                .GetAllAsync(c => c.Property, c => c.Tenant);

            return entities.Select(MapToDto);
        }
        public async Task<ContractDTO> GetContractByIdAsync(int id)
        {
            var contract = await _unitOfWork.Contracts.GetAsync(id);
            return MapToDto(contract);
        }

        public async Task<ContractDTO> CreateContractAsync(ContractDTO dto)
        {
            // LOGIC TẠO: Luôn tạo hợp đồng với trạng thái Pending
            var contract = new Contract
            {
                PropertyId = dto.PropertyId,
                TenantId = dto.TenantId,
                RentAmount = dto.RentAmount,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = ContractStatus.Pending
            };

            await _unitOfWork.Contracts.AddAsync(contract);
            await _unitOfWork.CompleteAsync();

            dto.Id = contract.Id;

            // --- GỬI THÔNG BÁO SIGNALR: Hợp đồng mới cần kích hoạt ---
            await _notificationService.SendContractNotificationAsync(
                "Hợp đồng Mới Cần Kích hoạt",
                $"Hợp đồng ID {contract.Id} đã được tạo (Pending).",
                $"/Contracts/Edit/{contract.Id}");
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

        // --- CHỨC NĂNG NGHIỆP VỤ: Kích hoạt Hợp đồng (Landlord) ---
        public async Task ActivateContractAsync(int contractId)
        {
            var contract = (await _unitOfWork.Contracts.FindAsync(c => c.Id == contractId, c => c.Property)).FirstOrDefault();
            if (contract == null) throw new KeyNotFoundException($"Contract ID {contractId} not found.");
            if (contract.Status != ContractStatus.Pending) throw new InvalidOperationException("Hợp đồng chỉ có thể được kích hoạt khi ở trạng thái 'Pending'.");

            // 1. Cập nhật trạng thái hợp đồng
            contract.Status = ContractStatus.Active;

            // 2. Cập nhật Property.IsOccupied = true (hoàn tất TODO)
            if (contract.Property != null)
            {
                contract.Property.IsOccupied = true;
                // Không cần gọi Update vì EF sẽ track thay đổi
            }

            await _unitOfWork.CompleteAsync();

            // 3. Gửi thông báo SignalR cho Tenant
            await _notificationService.SendContractNotificationAsync(
                "Hợp đồng Đã Kích hoạt",
                $"Hợp đồng ID {contractId} của bạn đã được kích hoạt.",
                $"/Tenant/MyContracts");
        }

        // --- CHỨC NĂNG NGHIỆP VỤ: Lấy Hợp đồng theo Tenant ID ---
        public async Task<IEnumerable<ContractDTO>> GetContractsByTenantIdAsync(int tenantId)
        {
            var entities = await _unitOfWork.Contracts
                .FindAsync(c => c.TenantId == tenantId, c => c.Property, c => c.Tenant);

            return entities.Select(MapToDto);
        }
    }
}