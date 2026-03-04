using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Application.DTOs.Warehouses
{
    public class UpdateWarehouseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int Capacity { get; set; }

        // Cho phép đổi người phụ trách (admin-only)
        public Guid UserId { get; set; }
    }
}
