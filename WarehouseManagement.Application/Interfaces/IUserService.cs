using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Application.Comom;
using WarehouseManagement.Application.DTOs.Users;

namespace WarehouseManagement.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> RegisterAsync(UserRegisterDto dto);
        Task<AuthResponseDto> LoginAsync(UserLoginDto dto);
        Task AssignRoleAsync(Guid userId, string role);
        Task<UserDto?> GetUserByIdAsync(Guid id, ICurrentUserService currentUserService);
        Task<IEnumerable<UserDto?>> GetAllUsersAsync();
        Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto dto, ICurrentUserService currentUserId);
        Task<UserDto> ChangPassword(Guid id, ChangePasswordDto dto, ICurrentUserService currentUserId);
        Task DeleteUserAsync(Guid id);
    }
}
