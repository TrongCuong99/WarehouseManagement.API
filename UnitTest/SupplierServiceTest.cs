using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Application.DTOs.Supplier;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Application.Services;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Domain.Interfaces;
using static WarehouseManagement.Domain.Common.DomainException;

namespace UnitTest
{
    public class SupplierServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ISupplierRepository> _supplierRepoMock = new();

        private readonly SupplierService _supplierService;
        public SupplierServiceTest()
        {
            _supplierService = new SupplierService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        #region CreateSupplier Tests
        [Fact]
        public async Task CreateSupplier_ShouldReturnSucess_WhenDataValid()
        {
            //Arrange
            var createSupplierDto = new CreateSupplierDto()
            {
                Name = "Supplier A",
                ContactEmail = "supplier@email.com",
                PhoneNumber = "1234567890",
                Address = "123 Supplier St."
            };
            _unitOfWorkMock.Setup(u => u.Supplier).Returns(_supplierRepoMock.Object);
            _supplierRepoMock.Setup(r => r.GetByNameAsync(createSupplierDto.Name)).ReturnsAsync((Supplier?)null);
            _mapperMock.Setup(m => m.Map<SupplierDto>(It.IsAny<Supplier>()))
                .Returns((Supplier s) => new SupplierDto
                {
                    Name = createSupplierDto.Name,
                    ContactEmail = createSupplierDto.ContactEmail,
                    PhoneNumber = createSupplierDto.PhoneNumber,
                    Address = createSupplierDto.Address
                });

            //Act
            var result = await _supplierService.CreateAsync(createSupplierDto);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(createSupplierDto.Name, result.Name);
            Assert.Equal(createSupplierDto.ContactEmail, result.ContactEmail);
        }

        [Fact]
        public async Task CreateSupplier_ShouldThrowException_WhenSupplierExist()
        {
            //Arrange
            var createSupplierDto = new CreateSupplierDto()
            {
                Name = "Supplier A",
                ContactEmail = "supplier@email.com",
                PhoneNumber = "1234567890",
                Address = "123 Supplier St."
            };
            var supplier = new Supplier(
                "Supplier A",
                "supplier@email.com",
                "1234567891",
                "124 Supplier St."
            );
            _unitOfWorkMock.Setup(u => u.Supplier).Returns(_supplierRepoMock.Object);
            _supplierRepoMock.Setup(r => r.GetByNameAsync(createSupplierDto.Name)).ReturnsAsync(supplier);

            //Assert&&Act
            await Assert.ThrowsAsync<ConflictException>(() => _supplierService.CreateAsync(createSupplierDto));
        }
        #endregion

        #region DeleteSupplier Tests
        [Fact]
        public async Task DeleteSupplier_ShouldThrowException_WhenSupplierNotFound()
        {
            //Arrange
            var supplier = new Supplier("Supplier A",
                "supplier@email.com",
                "1234567891",
                "124 Supplier St.");
            _unitOfWorkMock.Setup(u => u.Supplier).Returns(_supplierRepoMock.Object);
            _supplierRepoMock.Setup(r => r.GetByIdAsync(Guid.NewGuid())).ReturnsAsync((Supplier?)null);

            //Act&&Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _supplierService.DeleteAsync(supplier.Id));
        }

        [Fact]
        public async Task DeleteSupplier_ShouldReturnSuccess_WhenSupplierExist()
        {
            //Arrange
            var supplier = new Supplier("Supplier A",
                "supplier@email.com",
                "1234567891",
                "124 Supplier St.");
            _unitOfWorkMock.Setup(u => u.Supplier).Returns(_supplierRepoMock.Object);
            _supplierRepoMock.Setup(r => r.GetByIdAsync(supplier.Id)).ReturnsAsync(supplier);

            //Act
            await _supplierService.DeleteAsync(supplier.Id);

            //Assert
            _supplierRepoMock.Verify(r => r.Delete(supplier), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
        #endregion

        #region GetAllSupplier Tests
        [Fact]
        public async Task GetAllSupplier_ShouldReturnSuccess_WhenSupplierExist()
        {
            //Arrange
            var supplier1 = new Supplier("Supplier A", "supplier1@email.com", "1234567890", "123 Supplier St.");
            var supplier2 = new Supplier("Supplier B", "supplier2@email.com", "1234567891", "124 Supplier St.");

            _unitOfWorkMock.Setup(u => u.Supplier).Returns(_supplierRepoMock.Object);
            _supplierRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync([supplier1, supplier2]);
            _mapperMock.Setup(m => m.Map<List<SupplierDto>>(It.IsAny<IEnumerable<Supplier>>()))
                .Returns([
                    new SupplierDto
                    {
                        Name = supplier1.Name,
                        ContactEmail = supplier1.ContactEmail,
                        PhoneNumber = supplier1.PhoneNumber,
                        Address = supplier1.Address
                    },
                    new SupplierDto
                    {
                        Name = supplier2.Name,
                        ContactEmail = supplier2.ContactEmail,
                        PhoneNumber = supplier2.PhoneNumber,
                        Address = supplier2.Address
                    }]);

            //Act
            var result = await _supplierService.GetAllAsync();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetAllSupplier_ShouldReturnEmpty_WhenSupplierNotExist()
        {
            //Arrange
            _unitOfWorkMock.Setup(u => u.Supplier).Returns(_supplierRepoMock.Object);
            _supplierRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync([]);
            _mapperMock.Setup(m => m.Map<List<SupplierDto>>(It.IsAny<IEnumerable<Supplier>>())).Returns([]);

            //Act
            var result = await _supplierService.GetAllAsync();

            //Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
        #endregion

        #region
        [Fact]
        public async Task GetSupplierById_ShouldThrowException_WhenSupplierNotFound()
        {
            //Arrange
            _unitOfWorkMock.Setup(u => u.Supplier).Returns(_supplierRepoMock.Object);
            _supplierRepoMock.Setup(r => r.GetByIdAsync(Guid.NewGuid())).ReturnsAsync((Supplier?)null);
            //Act&&Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _supplierService.GetByIdAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task GetSupplierById_ShouldReturnException_WhenSupplierIdExist()
        {
            //Arrange
            var supplier = new Supplier("Supplier A", "supplier1@email.com", "1234567890", "123 Supplier St.");
            _unitOfWorkMock.Setup(u => u.Supplier).Returns(_supplierRepoMock.Object);
            _supplierRepoMock.Setup(r => r.GetByIdAsync(supplier.Id)).ReturnsAsync(supplier);
            _mapperMock.Setup(m => m.Map<SupplierDto>(It.IsAny<Supplier>()))
                .Returns(new SupplierDto
                {
                    Name = supplier.Name,
                    ContactEmail = supplier.ContactEmail,
                    PhoneNumber = supplier.PhoneNumber,
                    Address = supplier.Address
                });

            //Act
            var result = await _supplierService.GetByIdAsync(supplier.Id);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(supplier.Name, result.Name);
            Assert.Equal(supplier.ContactEmail, result.ContactEmail);
            Assert.Equal(supplier.PhoneNumber, result.PhoneNumber);
            Assert.Equal(supplier.Address, result.Address);
        }
        #endregion

        #region
        [Fact]
        public async Task UpdateSupplier_ShouldReturnSuccess_WhenSupplierIdExist()
        {
            //Arrange
            var updatesupplier = new UpdateSupplierDto() {Name = "Supplier B", ContactEmail = "supplierBBBB@email.com", PhoneNumber = "12345555666" , Address = "321 Supplier St." };
            var supplier = new Supplier("Supplier A", "supplier@email.com", "1234567890", "123 Supplier St.");
            _unitOfWorkMock.Setup(u => u.Supplier).Returns(_supplierRepoMock.Object);
            _supplierRepoMock.Setup(r => r.GetByIdAsync(supplier.Id)).ReturnsAsync(supplier);
            _mapperMock.Setup(m => m.Map<SupplierDto>(It.IsAny<Supplier>()))
                .Returns(new SupplierDto
                {
                    Name = updatesupplier.Name,
                    ContactEmail = updatesupplier.ContactEmail,
                    PhoneNumber = updatesupplier.PhoneNumber,
                    Address = updatesupplier.Address
                });

            //Act
            var result = await _supplierService.UpdateAsync(supplier.Id, updatesupplier);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(updatesupplier.Name, result.Name);
            Assert.Equal(updatesupplier.ContactEmail, result.ContactEmail);
            Assert.Equal(updatesupplier.PhoneNumber, result.PhoneNumber);
            Assert.Equal(updatesupplier.Address, result.Address);
        }

        [Fact]
        public async Task UpdateSupplier_ShouldThrowException_WhenSupplierIdNotExist()
        {
            //Arrange
            var supplier = new Supplier("Supplier A", "supplier@email.com", "1234567890", "123 Supplier St.");
            _unitOfWorkMock.Setup(u => u.Supplier).Returns(_supplierRepoMock.Object);
            _supplierRepoMock.Setup(r => r.GetByIdAsync(Guid.NewGuid())).ReturnsAsync((Supplier?)null);

            //Act&&Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _supplierService.UpdateAsync(supplier.Id, new UpdateSupplierDto()));
        }

        [Fact]
        public async Task UpdateSupplier_ShouldReturnSuccess_WhenUpdatePartialData()
        {
            //Arrange
            var updatesupplier = new UpdateSupplierDto() { Name = "Supplier B", ContactEmail = "supplierBBBB@email.com"};
            var supplier = new Supplier("Supplier A", "supplier@email.com", "1234567890", "123 Supplier St.");
            _unitOfWorkMock.Setup(u => u.Supplier).Returns(_supplierRepoMock.Object);
            _supplierRepoMock.Setup(r => r.GetByIdAsync(supplier.Id)).ReturnsAsync(supplier);
            _mapperMock.Setup(m => m.Map<SupplierDto>(It.IsAny<Supplier>()))
                .Returns(new SupplierDto
                {
                    Name = updatesupplier.Name,
                    ContactEmail = updatesupplier.ContactEmail,
                    PhoneNumber = supplier.PhoneNumber,
                    Address = supplier.Address
                });

            //Act
            var result = await _supplierService.UpdateAsync(supplier.Id, updatesupplier);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(updatesupplier.Name, result.Name);
            Assert.Equal(updatesupplier.ContactEmail, result.ContactEmail);
            Assert.Equal(supplier.PhoneNumber, result.PhoneNumber);
            Assert.Equal(supplier.Address, result.Address);
        }
        #endregion
    }
}
