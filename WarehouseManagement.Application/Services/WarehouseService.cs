using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Application.Common.Extensions;
using WarehouseManagement.Application.Comom;
using WarehouseManagement.Application.DTOs.Products;
using WarehouseManagement.Application.DTOs.Warehouses;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Application.Shared;
using WarehouseManagement.Domain.Entities;
using static WarehouseManagement.Domain.Common.DomainException;

namespace WarehouseManagement.Application.Services
{
    public class WarehouseService(IUnitOfWork unitOfWork, IMapper mapper) : IWarehouseService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        public async Task<WarehouseDto> CreateAsync(CreateWarehouseDto dto)
        {
            var existingWarehouse = await _unitOfWork.Warehouses.ExistsByNameAsync(dto.Name);
            if (existingWarehouse)
            {
                throw new ConflictException("Warehouse with the same name already exists");
            }

            var userExist = await _unitOfWork.User.GetByIdAsync(dto.UserId) ?? throw new KeyNotFoundException("User with ID does not exist");
            var warehouse = new Warehouse(dto.Name, dto.Location, dto.Capacity, dto.UserId);
            await _unitOfWork.Warehouses.AddAsync(warehouse);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<WarehouseDto>(warehouse);
        }

        public async Task DeleteAsync(int id)
        {
            var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(id) ?? throw new KeyNotFoundException("Warehouse with ID does not exist");
            _unitOfWork.Warehouses.Delete(warehouse);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IQueryable<WarehouseDto?>> GetAllAsync()
        {
            var warehouses = _unitOfWork.Warehouses.GetAllAsync() ?? throw new KeyNotFoundException("Warehouses does not exist");
            var result = await warehouses.ToPagedListAsync(1, 10);
            return _mapper.Map<IQueryable<WarehouseDto?>>(result);
        }

        public async Task<WarehouseDto?> GetByIdAsync(int id)
        {
            var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(id);
            if (warehouse != null)
            {
                return _mapper.Map<WarehouseDto>(warehouse);
            }
            throw new KeyNotFoundException("Warehouse with ID does not exist");
        }

        public async Task<WarehouseDto> UpdateAsync(int id, UpdateWarehouseDto dto)
        {
            var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(id) ?? throw new KeyNotFoundException("Warehouse not found");

            var WarehouseNameExist = await _unitOfWork.Warehouses.ExistsByNameAsync(dto.Name);
            if(!WarehouseNameExist)
            {
                warehouse.Name = dto.Name;
            }
            else
            {
                throw new ConflictException("Warehouse with the same name already exists");
            }
            if(!string.IsNullOrEmpty(dto.Location))
                warehouse.Location = dto.Location;

            if (dto.Capacity > 0)
                warehouse.Capacity = dto.Capacity;

            var userExist = await _unitOfWork.User.GetByIdAsync(dto.UserId);
            if (dto.UserId == 0)
            {
                warehouse.UserId = warehouse.UserId;
            }
            else if (dto.UserId > 0)
            {
                if(userExist == null)
                throw new KeyNotFoundException("User with ID does not exist");
                warehouse.UserId = dto.UserId;
            }
            else
            {
                throw new KeyNotFoundException("UserId must be greater than zero");
            }

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<WarehouseDto>(warehouse);
        }
    }
}
