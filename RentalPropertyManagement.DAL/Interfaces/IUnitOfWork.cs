using System;

namespace RentalPropertyManagement.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Sau này sẽ thêm các repository cụ thể ở đây, ví dụ:
        // IContractRepository Contracts { get; }
        // ITenantRepository Tenants { get; }
        IContractRepository Contracts { get; }
        void Save();
    }
}