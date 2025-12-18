using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.DAL.Entities;
using RentalPropertyManagement.DAL.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RentalPropertyManagement.BLL.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(RegisterDto registerDto);
        Task<User?> LoginAsync(LoginDto loginDto);

        // Lấy người dùng theo vai trò bất kỳ (Dùng cho trang Assign thợ)
        Task<IEnumerable<UserDto>> GetUsersByRoleAsync(UserRole role);

        // Lấy danh sách người thuê (Dùng cho trang tạo Hợp đồng)
        Task<IEnumerable<UserDto>> GetTenantsForSelectionAsync();
    }
}