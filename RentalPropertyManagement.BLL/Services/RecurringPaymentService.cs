using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using RentalPropertyManagement.DAL.Enums;
using RentalPropertyManagement.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.BLL.Services
{
    /// <summary>
    /// Service để tạo hóa đơn thanh toán hàng tháng cho các hợp đồng hoạt động
    /// </summary>
    public class RecurringPaymentService : IRecurringPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentInvoiceService _paymentInvoiceService;

        public RecurringPaymentService(IUnitOfWork unitOfWork, IPaymentInvoiceService paymentInvoiceService)
        {
            _unitOfWork = unitOfWork;
            _paymentInvoiceService = paymentInvoiceService;
        }

        /// <summary>
        /// Tạo hóa đơn thanh toán cho tháng hiện tại cho tất cả các hợp đồng Active
        /// Được gọi hàng tháng qua Hangfire
        /// </summary>
        public async Task CreateMonthlyInvoicesAsync()
        {
            try
            {
                var today = DateTime.Now;
                var currentMonth = new DateTime(today.Year, today.Month, 1);
                var nextMonth = currentMonth.AddMonths(1);
                var dueDate = nextMonth.AddDays(-1); // Hết hạn cuối cùng của tháng

                // Lấy tất cả các hợp đồng ACTIVE
                var activeContracts = await _unitOfWork.Contracts
                    .FindAsync(c => c.Status == ContractStatus.Active);

                foreach (var contract in activeContracts)
                {
                    // Kiểm tra xem tháng này đã có hóa đơn chưa
                    var existingInvoice = await _unitOfWork.PaymentInvoices
                        .FindAsync(pi =>
                            pi.ContractId == contract.Id &&
                            pi.InvoiceDate.Year == today.Year &&
                            pi.InvoiceDate.Month == today.Month
                        );

                    if (existingInvoice.Any())
                    {
                        continue; // Bỏ qua nếu đã có hóa đơn cho tháng này
                    }

                    // Tạo hóa đơn mới
                    var invoiceDto = new CreatePaymentInvoiceDTO
                    {
                        ContractId = contract.Id,
                        TenantId = contract.TenantId,
                        Amount = contract.RentAmount,
                        DueDate = dueDate,
                        Description = $"Tiền thuê tháng {currentMonth:MM/yyyy} - Hợp đồng #{contract.Id}"
                    };

                    try
                    {
                        await _paymentInvoiceService.CreateInvoiceAsync(invoiceDto);
                        System.Diagnostics.Debug.WriteLine($"✅ Tạo hóa đơn cho hợp đồng {contract.Id} - Tháng {currentMonth:MM/yyyy}");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ Lỗi tạo hóa đơn cho hợp đồng {contract.Id}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Lỗi trong CreateMonthlyInvoicesAsync: {ex.Message}");
            }
        }

        /// <summary>
        /// Tạo hóa đơn thanh toán cho một hợp đồng cụ thể
        /// </summary>
        public async Task CreateInvoiceForContractAsync(int contractId)
        {
            try
            {
                var contract = await _unitOfWork.Contracts.GetByIdAsync(contractId);
                if (contract == null || contract.Status != ContractStatus.Active)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ Hợp đồng {contractId} không tồn tại hoặc không Active");
                    return;
                }

                var today = DateTime.Now;
                var currentMonth = new DateTime(today.Year, today.Month, 1);
                var nextMonth = currentMonth.AddMonths(1);
                var dueDate = nextMonth.AddDays(-1);

                var invoiceDto = new CreatePaymentInvoiceDTO
                {
                    ContractId = contractId,
                    TenantId = contract.TenantId,
                    Amount = contract.RentAmount,
                    DueDate = dueDate,
                    Description = $"Tiền thuê tháng {currentMonth:MM/yyyy} - Hợp đồng #{contractId}"
                };

                await _paymentInvoiceService.CreateInvoiceAsync(invoiceDto);
                System.Diagnostics.Debug.WriteLine($"✅ Tạo hóa đơn cho hợp đồng {contractId}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Lỗi: {ex.Message}");
            }
        }
    }
}
