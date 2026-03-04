using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Application.DTOs.Users
{
    public class UpdateUserDto
    {
        public string? Role { get; set; }
        public string? Email { get; set; }
    }
}
