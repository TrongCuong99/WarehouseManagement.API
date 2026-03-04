using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Application.DTOs.Warehouses
{
    public class WarehouseViewModel
    {
        public Guid Id { get; set; }
        public string Location { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty; // map từ User.Email

        // Tùy chọn: tổng số lượng hàng tồn
        public int TotalStockItems { get; set; }
    }
}
