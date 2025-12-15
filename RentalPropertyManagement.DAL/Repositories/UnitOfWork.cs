using RentalPropertyManagement.DAL.Data;
using RentalPropertyManagement.DAL.Interfaces;

namespace RentalPropertyManagement.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RentalDbContext _context;

        // Sau này sẽ thêm các repository private fields ở đây
        private IContractRepository _contracts;

        public UnitOfWork(RentalDbContext context)
        {
            _context = context;
        }
        public IContractRepository Contracts
        {
            get
            {
                return _contracts ??= new ContractRepository(_context);
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}