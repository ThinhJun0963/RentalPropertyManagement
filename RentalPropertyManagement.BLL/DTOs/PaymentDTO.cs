using System;

namespace RentalPropertyManagement.BLL.DTOs
{
    public class PaymentDTO
    {
        public int Id { get; set; }
        public int PaymentInvoiceId { get; set; }
        public int TenantId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; }  // Nullable
        public string? TransactionId { get; set; }  // Nullable
        public string? ResponseCode { get; set; }  // Nullable
        public string? ResponseMessage { get; set; }  // Nullable
        public int Status { get; set; }  // 1=Pending, 2=Processing, 3=Completed, 4=Failed, 5=Cancelled, 6=Refunded

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

    public class CreatePaymentDTO
    {
        public int PaymentInvoiceId { get; set; }
        public int TenantId { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; }
    }

    public class VNPayPaymentDTO
    {
        public int PaymentInvoiceId { get; set; }
        public string? ReturnUrl { get; set; }
        public string? IpAddress { get; set; }
    }

    public class VNPayCallbackDTO
    {
        public string? vnp_Amount { get; set; }
        public string? vnp_BankCode { get; set; }
        public string? vnp_BankTranNo { get; set; }
        public string? vnp_CardType { get; set; }
        public string? vnp_OrderInfo { get; set; }
        public string? vnp_PayDate { get; set; }
        public string? vnp_ResponseCode { get; set; }
        public string? vnp_TmnCode { get; set; }
        public string? vnp_TransactionNo { get; set; }
        public string? vnp_TransactionStatus { get; set; }
        public string? vnp_TxnRef { get; set; }
        public string? vnp_SecureHash { get; set; }
        public string? vnp_SecureHashType { get; set; }
    }
}
