using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Application.DTOs.Products
{
    public class ProductSimpleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
