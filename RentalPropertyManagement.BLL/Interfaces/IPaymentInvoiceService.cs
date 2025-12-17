using System.Collections.Generic;
using System.Threading.Tasks;
using RentalPropertyManagement.BLL.DTOs;

namespace RentalPropertyManagement.BLL.Interfaces
{
    public interface IPaymentInvoiceService
    {
        Task<PaymentInvoiceDTO> CreateInvoiceAsync(CreatePaymentInvoiceDTO dto);
        Task<PaymentInvoiceDTO> GetInvoiceByIdAsync(int id);
        Task<IEnumerable<PaymentInvoiceDTO>> GetInvoicesByContractAsync(int contractId);
        Task<IEnumerable<PaymentInvoiceDTO>> GetInvoicesByTenantAsync(int tenantId);
        Task<bool> UpdateInvoiceStatusAsync(int id, string status);
        Task<bool> DeleteInvoiceAsync(int id);
    }
}
