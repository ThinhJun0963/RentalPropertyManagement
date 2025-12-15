using RentalPropertyManagement.BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RentalPropertyManagement.BLL.Interfaces
{
    public interface IPropertyService
    {
        // Lấy danh sách tài sản chưa có hợp đồng thuê
        Task<IEnumerable<PropertyDTO>> GetAvailablePropertiesForSelectionAsync();
    }
}