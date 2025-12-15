using System;
using System.ComponentModel.DataAnnotations;

namespace RentalPropertyManagement.DAL.Models
{
    public class Contract
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string TenantName { get; set; }

        [Required]
        [MaxLength(500)]
        public string PropertyAddress { get; set; }

        public decimal MonthlyRent { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}