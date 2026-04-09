using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Application.Comom;
using WarehouseManagement.Application.DTOs.Warehouses;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Application.Services;
using WarehouseManagement.Domain.Common;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Domain.Interfaces;
using static WarehouseManagement.Domain.Common.DomainException;

namespace UnitTest
{
    public class WarehouseServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<IWarehouseRepository> _warehouseRepoMock = new();
        private readonly Mock<IUserRepository> _userRepoMock = new();

        private readonly WarehouseService _warehouseService;

        public WarehouseServiceTest()
        {
            _warehouseService = new WarehouseService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        #region Delete Warehouse Test
        [Fact]
        public async Task DeleteWarehouse_ShouldReturnSuccess_WhenWarehouseIdExist()
        {
            // Arrange
            var warehouse = new Warehouse("Da Nang", "123 NVC", 1000, Guid.NewGuid());
            _unitOfWorkMock.Setup(uow => uow.Warehouses).Returns(_warehouseRepoMock.Object);
            _warehouseRepoMock.Setup(repo => repo.GetByIdAsync(warehouse.Id)).ReturnsAsync(warehouse);

            // Act
            await _warehouseService.DeleteAsync(warehouse.Id);

            // Assert
            _warehouseRepoMock.Verify(repo => repo.Delete(warehouse), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteWarehouse_ShouldThrowException_WhenWarehouseIdNotExist()
        {
            // Arrange
            var warehouse = new Warehouse("Da Nang", "123 NVC", 1000, Guid.NewGuid());
            _unitOfWorkMock.Setup(uow => uow.Warehouses).Returns(_warehouseRepoMock.Object);
            _warehouseRepoMock.Setup(repo => repo.GetByIdAsync(Guid.NewGuid())).ReturnsAsync((Warehouse?)null);

            //Act&&Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _warehouseService.DeleteAsync(warehouse.Id));
        }
        #endregion

        #region Get All Warehouse Test
        [Fact]
        public async Task GetAllWarehouses_ShouldReturnSuccess_WhenWarehouseExist()
        {
            // Arrange
            var warehouses = new List<Warehouse>
            {
                new("Da Nang", "123 NVC", 1000, Guid.NewGuid()),
                new("Hue", "456 ABC", 2000, Guid.NewGuid())
            };
            _unitOfWorkMock.Setup(uow => uow.Warehouses).Returns(_warehouseRepoMock.Object);
            _warehouseRepoMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(warehouses);
            var warehouseDtos = new List<WarehouseDto?>
            {
                new() { Name = "Da Nang", Location = "123 NVC", Capacity = 1000 },
                new() { Name = "Hue", Location = "456 ABC", Capacity = 2000 }
            };
            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<WarehouseDto?>>(warehouses)).Returns(warehouseDtos);

            // Act
            var result = await _warehouseService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllWarehouses_ShouldListEmpty_WhenWarehouseNotExist()
        {
            // Arrange
            _unitOfWorkMock.Setup(uow => uow.Warehouses).Returns(_warehouseRepoMock.Object);
            _warehouseRepoMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync([]);

            // Act
            var result = await _warehouseService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
        #endregion

        #region Get Warehouse by Id Test
        [Fact]
        public async Task GetWarehousebyId_ShouldReturnSuccess_WhenWarehouseIdExist()
        {
            // Arrange
            var warehouses = new Warehouse("Da Nang", "123 NVC", 1000, Guid.NewGuid());

            _unitOfWorkMock.Setup(uow => uow.Warehouses).Returns(_warehouseRepoMock.Object);
            _warehouseRepoMock.Setup(repo => repo.GetByIdAsync(warehouses.Id)).ReturnsAsync(warehouses);
            _mapperMock.Setup(mapper => mapper.Map<WarehouseDto?>(warehouses)).Returns(new WarehouseDto
            {
                Name = warehouses.Name,
                Location = warehouses.Location,
                Capacity = warehouses.Capacity,
                Id = warehouses.UserId
            });

            // Act
            var result = await _warehouseService.GetByIdAsync(warehouses.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(warehouses.Name, result!.Name);
            Assert.Equal(warehouses.Location, result.Location);
            Assert.Equal(warehouses.Capacity, result.Capacity);
            Assert.Equal(warehouses.UserId, result.Id);
        }

        [Fact]
        public async Task GetWarehousebyId_ShouldThrowException_WhenWarehouseIdNotExist()
        {
            // Arrange
            _unitOfWorkMock.Setup(uow => uow.Warehouses).Returns(_warehouseRepoMock.Object);
            _warehouseRepoMock.Setup(repo => repo.GetByIdAsync(Guid.NewGuid())).ReturnsAsync((Warehouse?)null);

            //Act&&Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _warehouseService.GetByIdAsync(Guid.NewGuid()));
        }
        #endregion

        #region Create Warehouse Test
        [Fact]
        public async Task CreateWarehouse_ShouldReturnSuccess_WhenWarehouseNameNotExist()
        {
            // Arrange
            var user = new User("test@gmail.com", "123546");

            var warehouseDto = new CreateWarehouseDto
            {
                Name = "Da Nang",
                Location = "123 NVC",
                Capacity = 1000,
                UserId = user.Id
            };

            _unitOfWorkMock.Setup(u => u.User).Returns(_userRepoMock.Object);
            _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _unitOfWorkMock.Setup(uow => uow.Warehouses).Returns(_warehouseRepoMock.Object);
            _warehouseRepoMock.Setup(repo => repo.ExistsByNameAsync(warehouseDto.Name)).ReturnsAsync(false);
            _mapperMock.Setup(mapper => mapper.Map<WarehouseDto>(It.IsAny<Warehouse>())).Returns(new WarehouseDto
            {
                Name = warehouseDto.Name,
                Location = warehouseDto.Location,
                Capacity = warehouseDto.Capacity,
                Id = warehouseDto.UserId
            });

            // Act
            var result = await _warehouseService.CreateAsync(warehouseDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(warehouseDto.Name, result.Name);
            Assert.Equal(warehouseDto.Location, result.Location);
            Assert.Equal(warehouseDto.Capacity, result.Capacity);
            Assert.Equal(warehouseDto.UserId, result.Id);
        }

        [Fact]
        public async Task CreateWarehouse_ShouldThrowException_WhenWarehouseNameExist()
        {
            // Arrange
            var warehouseDto = new CreateWarehouseDto
            {
                Name = "Da Nang",
                Location = "123 NVC",
                Capacity = 1000,
                UserId = Guid.NewGuid()
            };
            _unitOfWorkMock.Setup(uow => uow.Warehouses).Returns(_warehouseRepoMock.Object);
            _warehouseRepoMock.Setup(repo => repo.ExistsByNameAsync(warehouseDto.Name)).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(async () => await _warehouseService.CreateAsync(warehouseDto));
        }
        #endregion

        #region Update Warehouse Test
        [Fact]
        public async Task UpdateWarehouse_ShouldReturnSuccess_WhenWarehouseIdExist()
        {
            // Arrange
            var user = new User("test@gmail.com", "123546");
            var warehouse = new Warehouse("Da Nang", "123 NVC", 1000, user.Id);
            var updateDto = new UpdateWarehouseDto
            {
                Name = "Ha Noi",
                Location = "456 ABC",
                Capacity = 2000,
                UserId = user.Id
            };

            _unitOfWorkMock.Setup(u => u.User).Returns(_userRepoMock.Object);
            _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _unitOfWorkMock.Setup(uow => uow.Warehouses).Returns(_warehouseRepoMock.Object);
            _warehouseRepoMock.Setup(repo => repo.GetByIdAsync(warehouse.Id)).ReturnsAsync(warehouse);
            _warehouseRepoMock.Setup(repo => repo.ExistsByNameAsync(warehouse.Name)).ReturnsAsync(false);
            _mapperMock.Setup(mapper => mapper.Map<WarehouseDto>(warehouse)).Returns(new WarehouseDto
            {
                Name = updateDto.Name,
                Location = updateDto.Location,
                Capacity = updateDto.Capacity,
                Id = updateDto.UserId
            });

            // Act
            var result = await _warehouseService.UpdateAsync(warehouse.Id, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updateDto.Name, result.Name);
            Assert.Equal(updateDto.Location, result.Location);
            Assert.Equal(updateDto.Capacity, result.Capacity);
            Assert.Equal(updateDto.UserId, result.Id);
        }

        [Fact]
        public async Task UpdateWarehouse_ShouldThrowException_WhenWarehouseIdNotExist()
        {
            // Arrange
            var updateDto = new UpdateWarehouseDto
            {
                Name = "Da Nang",
                Location = "456 ABC",
                Capacity = 2000,
                UserId = Guid.NewGuid()
            };
            _unitOfWorkMock.Setup(uow => uow.Warehouses).Returns(_warehouseRepoMock.Object);
            _warehouseRepoMock.Setup(repo => repo.GetByIdAsync(Guid.NewGuid())).ReturnsAsync((Warehouse?)null);
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _warehouseService.UpdateAsync(Guid.NewGuid(), updateDto));
        }

        [Fact]
        public async Task UpdateWarehouse_ShouldThrowException_WhenWarehouseNameExist()
        {
            // Arrange
            var warehouse = new Warehouse("Da Nang", "123 NVC", 1000, Guid.NewGuid());
            var updateDto = new UpdateWarehouseDto
            {
                Name = "Da Nang",
                Location = "456 ABC",
                Capacity = 2000,
                UserId = Guid.NewGuid()
            };
            _unitOfWorkMock.Setup(uow => uow.Warehouses).Returns(_warehouseRepoMock.Object);
            _warehouseRepoMock.Setup(repo => repo.GetByIdAsync(warehouse.Id)).ReturnsAsync(warehouse);
            _warehouseRepoMock.Setup(repo => repo.ExistsByNameAsync(warehouse.Name)).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(async () => await _warehouseService.UpdateAsync(warehouse.Id, updateDto));
        }
        #endregion
    }
}
