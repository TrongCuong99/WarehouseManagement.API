using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Application.DTOs.Categories
{
    public class CreateCategoryDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}
