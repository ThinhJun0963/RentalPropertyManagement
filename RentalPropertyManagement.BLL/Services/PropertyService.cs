using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
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

        public async Task<IEnumerable<PropertyDTO>> GetAvailablePropertiesForSelectionAsync()
        {
            // Lấy properties chưa occupied (không có hợp đồng active)
            var properties = await _unitOfWork.Properties.FindAsync(p => !p.IsOccupied);
            return properties.Select(p => new PropertyDTO
            {
                Id = p.Id,
                Address = p.Address,
                City = p.City,
                IsOccupied = p.IsOccupied
            });
        }
    }
}