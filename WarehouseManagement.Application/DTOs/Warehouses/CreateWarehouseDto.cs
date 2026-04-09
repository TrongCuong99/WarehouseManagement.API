using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Application.DTOs.Warehouses
{
    public class CreateWarehouseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int Capacity { get; set; }

        // Nếu bạn muốn cho admin gán kho cho user cụ thể
        public Guid UserId { get; set; }
    }
}
