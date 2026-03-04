using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Application.Comom;
using WarehouseManagement.Application.DTOs.Products;
using WarehouseManagement.Application.DTOs.Warehouses;
using WarehouseManagement.Application.Shared;

namespace WarehouseManagement.Application.Interfaces
{
    public interface IWarehouseService
    {
        Task<WarehouseDto> CreateAsync(CreateWarehouseDto dto);
        Task<IEnumerable<WarehouseDto?>> GetAllAsync();
        Task<WarehouseDto?> GetByIdAsync(Guid id);
        Task<WarehouseDto> UpdateAsync(Guid id, UpdateWarehouseDto dto);
        Task DeleteAsync(Guid id);
    }
}
