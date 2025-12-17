using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentalPropertyManagement.DAL.Entities
{
    public class PaymentInvoice
    {
        public int Id { get; set; }

        // Foreign Keys
        public int ContractId { get; set; }
        public int TenantId { get; set; }

        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? PaidDate { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        public string? Description { get; set; }
        public Enums.PaymentStatus Status { get; set; }
        public string? TransactionReference { get; set; } // VNPay Transaction ID

        // Navigation properties
        public virtual Contract Contract { get; set; }
        public virtual User Tenant { get; set; }
    }
}
