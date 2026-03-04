using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Application.DTOs.Supplier
{
    public class UpdateSupplierDto
    {
        public string? Name { get; set; }
        public string? ContactEmail { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
    }
}
