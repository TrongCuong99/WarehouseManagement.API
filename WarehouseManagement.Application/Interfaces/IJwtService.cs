using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Application.Interfaces
{
    public interface IJwtService
    {
        public (string, DateTime) GenerateToken(Guid userId, string email, string role);
        public string GenerateRefreshToken();
    }
}
