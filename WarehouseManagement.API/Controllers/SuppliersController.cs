using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Application.DTOs.Supplier;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Application.Shared;
using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.API.Controllers
{
    [ApiController]
    [Route("api/supplier")]
    public class SuppliersController(ISupplierService service) : ControllerBase
    {
        private readonly ISupplierService _service = service;

        [HttpGet]
        public async Task<ApiResponse<List<SupplierDto>>> GetAll()
        {
            var suppliers = await _service.GetAllAsync();
            return new ApiResponse<List<SupplierDto>>(200, "Get Suppliers Succesful", suppliers);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var supplier = await _service.GetByIdAsync(id);
            if (supplier != null)
            {
                return Ok(new ApiResponse<SupplierDto>(200, "Get Supplier with Id Successful", supplier));
            }
            else
            {
                return NotFound(new ApiResponse<SupplierDto>(404, "Not Found Supplier with Id"));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateSupplierDto dto)
        {
            var createdSupplier = await _service.CreateAsync(dto);
            if (createdSupplier != null)
            {
                return Ok(new ApiResponse<SupplierDto>(200, "Get Supplier with Id Successful", createdSupplier));
            }
            else
            {
                return BadRequest(new ApiResponse<SupplierDto>(400, "Create Supplier Failed"));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateSupplierDto dto)
        {
            var updatedSupplier = await _service.UpdateAsync(id, dto);
            if (updatedSupplier != null)
            {
                return Ok(new ApiResponse<SupplierDto>(200, "Update Supplier Successful", updatedSupplier));
            }
            else
            {
                return BadRequest(new ApiResponse<SupplierDto>(400, "Update Supplier Failed"));
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return Ok(new ApiResponse<SupplierDto>(200, "Delete Supplier Successful"));
        }
    }
}
