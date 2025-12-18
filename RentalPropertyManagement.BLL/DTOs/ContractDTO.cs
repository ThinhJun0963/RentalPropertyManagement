using System;
using RentalPropertyManagement.DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace RentalPropertyManagement.BLL.DTOs
{
    public class ContractDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn tài sản.")]
        public int PropertyId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn người thuê.")]
        public int TenantId { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số tiền thuê.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Tiền thuê phải lớn hơn 0.")]
        public decimal RentAmount { get; set; }

        public ContractStatus Status { get; set; }

        // SỬA LỖI: Thêm dấu ? để biến các trường này thành tùy chọn khi xác thực
        public string? TenantName { get; set; }
        public string? PropertyAddress { get; set; }
    }
}