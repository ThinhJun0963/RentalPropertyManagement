using System;
using RentalPropertyManagement.DAL.Enums;

namespace RentalPropertyManagement.BLL.DTOs
{
    public class MaintenanceRequestDTO
    {
        public int Id { get; set; }

        public int TenantId { get; set; }
        // Sử dụng đúng tên class UserDto (chữ d viết thường)
        public UserDto? Tenant { get; set; }

        public int PropertyId { get; set; }
        public PropertyDTO? Property { get; set; }

        public int? AssignedProviderId { get; set; }
        public UserDto? AssignedProvider { get; set; }

        public string Description { get; set; } = string.Empty;
        public string? Title { get; set; } // Thêm Title để khớp với SQL
        public RequestPriority Priority { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime SubmittedDate { get; set; }
        public string? AttachmentUrl { get; set; }
    }
}