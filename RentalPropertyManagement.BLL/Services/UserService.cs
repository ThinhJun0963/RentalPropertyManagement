using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using RentalPropertyManagement.DAL.Entities;
using RentalPropertyManagement.DAL.Enums;
using RentalPropertyManagement.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;

namespace RentalPropertyManagement.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await _unitOfWork.Users.GetSingleAsync(u => u.Email == registerDto.Email);
            if (existingUser != null) return false;

            var user = new User
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                PhoneNumber = registerDto.PhoneNumber,
                Role = registerDto.Role
            };

            await _unitOfWork.Users.AddAsync(user);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<User?> LoginAsync(LoginDto loginDto)
        {
            var user = await _unitOfWork.Users.GetSingleAsync(u => u.Email == loginDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                return null;
            return user;
        }

        // --- Triển khai GetUsersByRoleAsync ---
        public async Task<IEnumerable<UserDto>> GetUsersByRoleAsync(UserRole role)
        {
            var users = await _unitOfWork.Users.FindAsync(u => u.Role == role);

            return users.Select(u => new UserDto
            {
                Id = u.Id,
                FullName = $"{u.FirstName} {u.LastName}",
                Email = u.Email,
                Role = u.Role.ToString()
            });
        }

        // --- Triển khai GetTenantsForSelectionAsync ---
        public async Task<IEnumerable<UserDto>> GetTenantsForSelectionAsync()
        {
            // Tận dụng hàm GetUsersByRoleAsync để lấy tất cả Tenant
            return await GetUsersByRoleAsync(UserRole.Tenant);
        }
    }
}