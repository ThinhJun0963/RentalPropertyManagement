using RentalPropertyManagement.DAL.Data;
using RentalPropertyManagement.DAL.Interfaces;
using RentalPropertyManagement.DAL.Models;

namespace RentalPropertyManagement.DAL.Repositories
{
    public class ContractRepository : Repository<Contract>, IContractRepository
    {
        public ContractRepository(RentalDbContext context) : base(context)
        {
        }
    }
}