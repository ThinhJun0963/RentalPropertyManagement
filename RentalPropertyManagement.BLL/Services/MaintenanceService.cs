using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using RentalPropertyManagement.DAL.Entities;
using RentalPropertyManagement.DAL.Enums;
using RentalPropertyManagement.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.BLL.Services
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public MaintenanceService(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        // 1. Lấy danh sách công việc cho Thợ (Giải quyết lỗi missing member)
        public async Task<IEnumerable<MaintenanceRequestDTO>> GetTasksByProviderIdAsync(int providerId)
        {
            // Cần Include Property và Tenant để DTO có dữ liệu hiển thị
            var entities = await _unitOfWork.MaintenanceRequests
                .FindAsync(r => r.AssignedProviderId == providerId, r => r.Property, r => r.Tenant);

            return entities.Select(MapToDto);
        }

        public async Task<MaintenanceRequestDTO> CreateRequestAsync(MaintenanceRequestDTO dto)
        {
            var request = new MaintenanceRequest
            {
                TenantId = dto.TenantId,
                PropertyId = dto.PropertyId,
                Description = dto.Description,
                Priority = dto.Priority,
                Status = RequestStatus.New,
                SubmittedDate = DateTime.Now,
                AttachmentUrl = dto.AttachmentUrl
            };

            await _unitOfWork.MaintenanceRequests.AddAsync(request);
            await _unitOfWork.CompleteAsync();

            await _notificationService.SendMaintenanceNotificationAsync("Yêu cầu mới", "Có yêu cầu bảo trì mới cần xử lý", "/Maintenance/Inbox");

            dto.Id = request.Id;
            return dto;
        }

        public async Task UpdateStatusAsync(int requestId, RequestStatus newStatus)
        {
            var request = await _unitOfWork.MaintenanceRequests.GetAsync(requestId);
            if (request != null)
            {
                request.Status = newStatus;
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task AssignProviderAsync(int requestId, int providerId)
        {
            var request = await _unitOfWork.MaintenanceRequests.GetAsync(requestId);
            if (request != null)
            {
                request.AssignedProviderId = providerId;
                request.Status = RequestStatus.Approved;
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<IEnumerable<MaintenanceRequestDTO>> GetAllRequestsAsync()
        {
            var entities = await _unitOfWork.MaintenanceRequests.GetAllAsync(r => r.Tenant, r => r.Property, r => r.AssignedProvider);
            return entities.Select(MapToDto);
        }

        public async Task<MaintenanceRequestDTO> GetRequestByIdAsync(int id)
        {
            var entities = await _unitOfWork.MaintenanceRequests.FindAsync(r => r.Id == id, r => r.Tenant, r => r.Property, r => r.AssignedProvider);
            return MapToDto(entities.FirstOrDefault());
        }

        public async Task<IEnumerable<MaintenanceRequestDTO>> GetRequestsByTenantIdAsync(int tenantId)
        {
            var entities = await _unitOfWork.MaintenanceRequests.FindAsync(r => r.TenantId == tenantId, r => r.Property);
            return entities.Select(MapToDto);
        }

        // --- Hàm Mapping chuẩn để không bị lỗi "Tenant" hay "Property" ---
        private MaintenanceRequestDTO MapToDto(MaintenanceRequest entity)
        {
            if (entity == null) return null;
            return new MaintenanceRequestDTO
            {
                Id = entity.Id,
                Description = entity.Description,
                Priority = entity.Priority,
                Status = entity.Status,
                SubmittedDate = entity.SubmittedDate,
                AttachmentUrl = entity.AttachmentUrl,

                TenantId = entity.TenantId,
                Tenant = entity.Tenant == null ? null : new UserDto
                {
                    Id = entity.Tenant.Id,
                    FullName = $"{entity.Tenant.FirstName} {entity.Tenant.LastName}",
                    Email = entity.Tenant.Email
                },

                PropertyId = entity.PropertyId,
                Property = entity.Property == null ? null : new PropertyDTO
                {
                    Id = entity.Property.Id,
                    Address = entity.Property.Address,
                    City = entity.Property.City,
                    MonthlyRent = entity.Property.MonthlyRent,
                    Description = entity.Property.Description
                },

                AssignedProviderId = entity.AssignedProviderId,
                AssignedProvider = entity.AssignedProvider == null ? null : new UserDto
                {
                    Id = entity.AssignedProvider.Id,
                    FullName = $"{entity.AssignedProvider.FirstName} {entity.AssignedProvider.LastName}"
                }
            };
        }
    }
}