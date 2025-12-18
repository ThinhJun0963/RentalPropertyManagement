using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentalPropertyManagement.DAL.Entities
{
    public class Payment
    {
        public int Id { get; set; }

        // Foreign Keys
        public int PaymentInvoiceId { get; set; }
        public int TenantId { get; set; }

        public DateTime PaymentDate { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        public string? PaymentMethod { get; set; } // VNPay, Stripe, etc.
        public string? TransactionId { get; set; } // VNPay transaction ID
        public string? ResponseCode { get; set; } // VNPay response code
        public string? ResponseMessage { get; set; } // VNPay response message
        
        public Enums.PaymentStatus Status { get; set; }

        // Navigation properties
        public virtual PaymentInvoice PaymentInvoice { get; set; }
        public virtual User Tenant { get; set; }
    }
}
