using RentalPropertyManagement.DAL.Models;

namespace RentalPropertyManagement.DAL.Interfaces
{
    public interface IContractRepository : IRepository<Contract>
    {
        // Sau này có thể thêm: IEnumerable<Contract> GetExpiringContracts(int days);
    }
}