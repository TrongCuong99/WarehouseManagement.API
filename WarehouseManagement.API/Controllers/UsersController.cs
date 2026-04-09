using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Application.Comom;
using WarehouseManagement.Application.DTOs.Users;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Application.Services;
using WarehouseManagement.Application.Shared;

namespace WarehouseManagement.API.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UsersController(IUserService service, IAuthService authService) : ControllerBase
    {
        private readonly IUserService _service = service;
        private readonly IAuthService _authService = authService;

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            var userDto = await _service.RegisterAsync(dto);
            if(userDto == null)
            {
                return Conflict(new ApiResponse<UserDto>(409, "Registration failed"));
            }
            return Ok(new ApiResponse<UserDto>(200, "Registration successful", userDto));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            var authResponse = await _service.LoginAsync(dto);
            return Ok(new ApiResponse<AuthResponseDto>(200, "Login successful", authResponse));
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost("assign-role/{id:guid}")]
        public async Task<IActionResult> AssignRole(Guid id, string role)
        {
            await _service.AssignRoleAsync(id, role);
            return Ok(new ApiResponse<string>(200, "Role assigned successfully"));
        }

        [Authorize]
        [HttpPost("update/{id:guid}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto dto, [FromServices] ICurrentUserService currentUserService)
        {
            var userDto = await _service.UpdateUserAsync(id, dto, currentUserService);
            return Ok(new ApiResponse<UserDto?>(200, "Update User Successfully", userDto));
        }

        [Authorize]
        [HttpPost("changepassword/{id:guid}")]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordDto dto, [FromServices] ICurrentUserService currentUserService)
        {
            var userDto = await _service.ChangPassword(id, dto, currentUserService);
            return Ok(new ApiResponse<UserDto?>(200, "Get User by Id Successfully", userDto));
        }

        [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUserById(Guid id, [FromServices] ICurrentUserService currentUserService)
        {
            var userDto = await _service.GetUserByIdAsync(id, currentUserService);
            if (userDto == null)
            {
                return NotFound(new ApiResponse<UserDto?>(404, "User not found"));
            }
            return Ok(new ApiResponse<UserDto?>(200, "Get User by Id Successfully", userDto));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _service.GetAllUsersAsync();
            return Ok(new ApiResponse<IEnumerable<UserDto?>>(200, "Get All Users Successfully", users));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await _service.DeleteUserAsync(id);
            return Ok(new ApiResponse<string>(200, "Delete User Successfully"));
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto dto)
        {
            var result = await _authService.RefreshTokenAsync(dto);
            return Ok(result);
        }
    }
}
