using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Application.DTOs.Users
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        //public int WarehouseCount { get; set; }
        //public int CreatedTransactionCount { get; set; }
        //public int ApprovedTransactionCount { get; set; }
    }
}
