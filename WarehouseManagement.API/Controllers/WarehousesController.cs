using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Application.Comom;
using WarehouseManagement.Application.DTOs.Warehouses;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Application.Shared;

namespace WarehouseManagement.API.Controllers
{
    [ApiController]
    [Route("api/warehouse")]
    public class WarehousesController(IWarehouseService service) : ControllerBase
    {
        private readonly IWarehouseService _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var warehouses = await _service.GetAllAsync();
            return Ok(new ApiResponse<IEnumerable<WarehouseDto?>>(200, "Get Warehouses Successfully", warehouses));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateWarehouseDto dto)
        {
            var warehouse = await _service.CreateAsync(dto);
            if (warehouse != null)
            {
                return Ok(new ApiResponse<WarehouseDto>(200, "Warehouse created successfully", warehouse));
            }
            return BadRequest(new ApiResponse<WarehouseDto>(400, "Warehouse creation failed"));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var warehouse = await _service.GetByIdAsync(id);
            if (warehouse != null)
            {
                return Ok(new ApiResponse<WarehouseDto>(200, "Get Warehouse by Id Successfully", warehouse));
            }
            return NotFound(new ApiResponse<WarehouseDto>(404, "Warehouse with ID does not exist"));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWarehouseDto dto)
        {
            var warehouse = await _service.UpdateAsync(id, dto);
            return Ok(new ApiResponse<WarehouseDto>(200, "Warehouse updated successfully", warehouse));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return Ok(new ApiResponse<WarehouseDto>(200, "Warehouse deleted successfully"));
        }
    }
}
