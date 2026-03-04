using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Application.DTOs.Users;

namespace WarehouseManagement.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto);
    }
}
