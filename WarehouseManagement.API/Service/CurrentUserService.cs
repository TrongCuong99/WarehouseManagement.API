using System.Security.Claims;
using WarehouseManagement.Application.Comom;

namespace WarehouseManagement.API.Service
{
    public class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = accessor;

        
        public Guid? UserId => Guid.TryParse(_httpContextAccessor.HttpContext?
                .User?
                .FindFirstValue(ClaimTypes.NameIdentifier), out var id)
                ? id : Guid.Empty;

        public string? Email =>_httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

        public string? Roles => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
    }
}
