using System;

namespace RentalPropertyManagement.BLL.DTOs
{
    public class ContractDTO
    {
        public int Id { get; set; }
        public string TenantName { get; set; }
        public string PropertyAddress { get; set; }
        public decimal MonthlyRent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }

        // Thêm các field khác sau này: Deposit, Terms, etc.
    }
}