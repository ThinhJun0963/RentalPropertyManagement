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
            // Lấy các tài sản chưa có người thuê
            var properties = await _unitOfWork.Properties.FindAsync(p => !p.IsOccupied);

            return properties.Select(p => new PropertyDTO
            {
                Id = p.Id,
                Address = p.Address,
                City = p.City,
                IsOccupied = p.IsOccupied,
                // Bây giờ các trường này sẽ không còn báo lỗi đỏ nữa
                SquareFootage = p.SquareFootage,
                MonthlyRent = p.MonthlyRent,
                Description = p.Description
            });
        }
    }
}