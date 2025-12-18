using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using RentalPropertyManagement.DAL.Entities;
using RentalPropertyManagement.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.BLL.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PropertyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // View: Get all properties for management
        public async Task<IEnumerable<PropertyDTO>> GetAllPropertiesAsync()
        {
            var properties = await _unitOfWork.Properties.GetAllAsync();
            return properties.Select(p => MapToDTO(p));
        }

        // View: Get specific property details
        public async Task<PropertyDTO> GetPropertyByIdAsync(int id)
        {
            var property = await _unitOfWork.Properties.GetByIdAsync(id);
            return property != null ? MapToDTO(property) : null;
        }

        // View: Existing method for tenant selection
        public async Task<IEnumerable<PropertyDTO>> GetAvailablePropertiesForSelectionAsync()
        {
            var properties = await _unitOfWork.Properties.FindAsync(p => !p.IsOccupied);
            return properties.Select(p => MapToDTO(p));
        }

        // Add: Create a new property record
        public async Task AddPropertyAsync(PropertyDTO propertyDto)
        {
            var property = new Property
            {
                Address = propertyDto.Address,
                City = propertyDto.City,
                SquareFootage = propertyDto.SquareFootage,
                MonthlyRent = propertyDto.MonthlyRent,
                Description = propertyDto.Description,
                IsOccupied = false // New properties are vacant by default
            };

            await _unitOfWork.Properties.AddAsync(property);
            await _unitOfWork.CompleteAsync();
        }

        // Update: Modify an existing property
        public async Task UpdatePropertyAsync(PropertyDTO propertyDto)
        {
            var property = await _unitOfWork.Properties.GetByIdAsync(propertyDto.Id);
            if (property != null)
            {
                property.Address = propertyDto.Address;
                property.City = propertyDto.City;
                property.SquareFootage = propertyDto.SquareFootage;
                property.MonthlyRent = propertyDto.MonthlyRent;
                property.Description = propertyDto.Description;
                property.IsOccupied = propertyDto.IsOccupied;

                _unitOfWork.Properties.Update(property);
                await _unitOfWork.CompleteAsync();
            }
        }

        // Delete: Remove a property from the system
        public async Task DeletePropertyAsync(int id)
        {
            var property = await _unitOfWork.Properties.GetByIdAsync(id);
            if (property != null)
            {
                _unitOfWork.Properties.Remove(property);
                await _unitOfWork.CompleteAsync();
            }
        }

        // Helper: Centralized mapping logic
        private PropertyDTO MapToDTO(Property p)
        {
            return new PropertyDTO
            {
                Id = p.Id,
                Address = p.Address,
                City = p.City,
                IsOccupied = p.IsOccupied,
                SquareFootage = p.SquareFootage,
                MonthlyRent = p.MonthlyRent,
                Description = p.Description
            };
        }
    }
}