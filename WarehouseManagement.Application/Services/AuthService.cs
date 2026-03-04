using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Application.DTOs.Users;
using WarehouseManagement.Application.Interfaces;

namespace WarehouseManagement.Application.Services
{
    public class AuthService(IUnitOfWork unitOfWork, IJwtService jwtService) : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IJwtService _jwtService = jwtService;

        public async Task<AuthResponseDto> RefreshTokenAsync(
            RefreshTokenRequestDto dto)
        {
            var user = await _unitOfWork.User.GetByIdAsync(dto.UserId) ?? throw new UnauthorizedAccessException();

            if (user.RefreshToken != dto.RefreshToken)
                throw new UnauthorizedAccessException();

            if (user.RefreshTokenExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Refresh token expired");

            var newRefreshToken = _jwtService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);

            var newAccessToken = _jwtService.GenerateToken(user.Id, user.Email, user.Role);

            await _unitOfWork.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = newAccessToken.Item1,
                RefreshToken = newRefreshToken
            };
        }
    }
}
