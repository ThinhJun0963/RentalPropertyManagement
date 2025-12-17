using System;

namespace RentalPropertyManagement.BLL.DTOs
{
    public class PaymentInvoiceDTO
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public int TenantId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }  // Nullable - can be NULL in database
        public int Status { get; set; }  // 1=Pending, 2=Processing, 3=Completed, 4=Failed, 5=Cancelled, 6=Refunded
        public string? TransactionReference { get; set; }  // Nullable - can be NULL in database

        // Helper property để display status text
        public string StatusText => Status switch
        {
            1 => "Pending",
            2 => "Processing",
            3 => "Completed",
            4 => "Failed",
            5 => "Cancelled",
            6 => "Refunded",
            _ => "Unknown"
        };
    }

    public class CreatePaymentInvoiceDTO
    {
        public int ContractId { get; set; }
        public int TenantId { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }
}
