using AutoMapper;
using StructureMap;
using WarehouseManagement.Application.Comom;
using WarehouseManagement.Application.DTOs.Users;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Domain.Entities;
using static WarehouseManagement.Domain.Common.DomainException;

namespace WarehouseManagement.Application.Services
{
    public class UserService(IUnitOfWork unitOfWork, IJwtService jwtService, IPasswordHasher passwordHasher, IMapper mapper) : IUserService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IJwtService _jwtService = jwtService;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly IMapper _mapper = mapper;

        public async Task<UserDto> RegisterAsync(UserRegisterDto dto)
        {
            var existingUser = await _unitOfWork.User.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new ConflictException("User with this email already exists");
            var user = new User(dto.Email, _passwordHasher.Hash(dto.Password));
            await _unitOfWork.User.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<UserDto>(user);
        }

        public async Task<AuthResponseDto> LoginAsync(UserLoginDto dto)
        {
            var user = await _unitOfWork.User.GetUserByEmailAsync(dto.Email)
                ?? throw new KeyNotFoundException("Email not found");

            if (!_passwordHasher.Verify(dto.Password, user.Password))
                throw new UnauthorizedAccessException("Invalid credentials");

            var token = _jwtService.GenerateToken(user.Id, user.Email, user.Role);

            var refreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);

            await _unitOfWork.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = token.Item1,
                AccessTokenExpiresAt = token.Item2,
                RefreshToken = refreshToken,
                RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7),
                Id = user.Id,
                Email = user.Email,
                Role = user.Role
            };
        }

        public async Task AssignRoleAsync(Guid userId, string role)
        {
            var user = await _unitOfWork.User.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException("User not found");
            user.AssignRole(role);
            _unitOfWork.User.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid id, ICurrentUserService currentUserService)
        {
            var user = await _unitOfWork.User.GetByIdAsync(id);
            if (user != null)
            {
                if (currentUserService.Roles == "Admin")
                {
                    return _mapper.Map<UserDto>(user);
                }

                if (currentUserService.UserId != id)
                {
                    throw new UnauthorizedAccessException("You are not allowed to access this user.");
                }
                return _mapper.Map<UserDto>(user);
            }
            throw new KeyNotFoundException("User with ID not exist");
        }

        public async Task<IEnumerable<UserDto?>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.User.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto dto, ICurrentUserService currentUserId)
        {
            var user = await _unitOfWork.User.GetByIdAsync(id) ?? throw new KeyNotFoundException("User with ID not exist");
            bool isAdmin = currentUserId.Roles == "Admin";

            if (user == null)
                throw new KeyNotFoundException("User not found");

            if (!isAdmin)
            {
                if (currentUserId.UserId == id)
                {
                    user.Email = dto.Email!;
                }
                else
                {
                    throw new UnauthorizedAccessException("You are not allowed to update this user.");
                }
            }
            else
            {
                user.Role = dto.Role!;
                user.Email = dto.Email!;
            }
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<UserDto>(user);
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await _unitOfWork.User.GetByIdAsync(id) ?? throw new KeyNotFoundException("User with ID not exist");
            _unitOfWork.User.Delete(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<UserDto> ChangPassword(Guid id, ChangePasswordDto dto, ICurrentUserService currentUserService)
        {
            var currentUser = currentUserService.UserId;
            bool isAdmin = currentUserService.Roles == "Admin";

            var user = await _unitOfWork.User.GetByIdAsync(id) ?? throw new KeyNotFoundException("User not found.");
            var valid = _passwordHasher.Verify(dto.CurrentPassword!, user.Password);
            if (!isAdmin)
            {
                if (currentUser != id)
                    throw new UnauthorizedAccessException("You cannot change another user's password.");

                if (string.IsNullOrEmpty(dto.CurrentPassword))
                    throw new UnauthorizedAccessException("Current password is required.");

                if (!valid)
                    throw new UnauthorizedAccessException("Current password is incorrect.");
            }

            user.UpdatePassword(_passwordHasher.Hash(dto.NewPassword));
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }
    }
}
