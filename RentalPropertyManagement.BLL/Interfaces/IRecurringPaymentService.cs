using System.Threading.Tasks;

namespace RentalPropertyManagement.BLL.Interfaces
{
    public interface IRecurringPaymentService
    {
        /// <summary>
        /// Tạo hóa đơn thanh toán hàng tháng cho tất cả các hợp đồng Active
        /// </summary>
        Task CreateMonthlyInvoicesAsync();

        /// <summary>
        /// Tạo hóa đơn thanh toán cho một hợp đồng cụ thể
        /// </summary>
        Task CreateInvoiceForContractAsync(int contractId);
    }
}
