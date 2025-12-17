using System.Collections.Generic;
using System.Threading.Tasks;
using RentalPropertyManagement.BLL.DTOs;

namespace RentalPropertyManagement.BLL.Interfaces
{
    public interface IPaymentService
    {
        Task<string> CreateVNPayPaymentUrlAsync(VNPayPaymentDTO dto);
        Task<PaymentDTO> ProcessVNPayCallbackAsync(VNPayCallbackDTO dto);
        Task<PaymentDTO> GetPaymentByIdAsync(int id);
        Task<IEnumerable<PaymentDTO>> GetPaymentsByInvoiceAsync(int invoiceId);
        Task<IEnumerable<PaymentDTO>> GetPaymentsByTenantAsync(int tenantId);
        Task<bool> UpdatePaymentStatusAsync(int id, string status);
    }
}
