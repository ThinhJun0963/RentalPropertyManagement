// RentalPropertyManagement.BLL/Interfaces/IPropertyService.cs
using RentalPropertyManagement.BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RentalPropertyManagement.BLL.Interfaces
{
    public interface IPropertyService
    {
        // View Available (Existing)
        Task<IEnumerable<PropertyDTO>> GetAvailablePropertiesForSelectionAsync();

        // New Management Methods for Landlord
        Task<IEnumerable<PropertyDTO>> GetAllPropertiesAsync();
        Task<PropertyDTO> GetPropertyByIdAsync(int id);
        Task AddPropertyAsync(PropertyDTO propertyDto);
        Task UpdatePropertyAsync(PropertyDTO propertyDto);
        Task DeletePropertyAsync(int id);
    }
}