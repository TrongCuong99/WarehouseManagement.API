using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Application.Comom;
using WarehouseManagement.Application.DTOs.WarehouseTransactions;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Application.Services;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Domain.Enums;
using WarehouseManagement.Domain.Interfaces;
using static WarehouseManagement.Domain.Common.DomainException;

namespace UnitTest
{
    public class WarehouseTransactionServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<IWarehouseTransactionRepository> _transactionRepoMock = new();
        private readonly Mock<IProductRepository> _productRepoMock = new();
        private readonly Mock<ICurrentUserService> _currentServiceMock = new();
        private readonly Mock<IWarehouseRepository> _warehouseRepoMock = new();
        private readonly Mock<IUserRepository> _userRepoMock = new();

        private readonly WarehouseTransactionService _transactionservice;

        public WarehouseTransactionServiceTest()
        {
            _transactionservice = new WarehouseTransactionService(_unitOfWorkMock.Object, Mock.Of<IStockService>(), _mapperMock.Object);
        }

        #region CreateTransactionAsync Tests
        [Fact]
        public async Task CreateTransaction_ShouldReturnSuccess_WhenDataValid()
        {
            //Arrange
            var productIds = new List<Product>
            {
                new("Product1", "Description1", "SKU1", 1) { Name = "Product1", Description = "Description1" },
                new("Product2", "Description2", "SKU2", 1) { Name = "Product2", Description = "Description2" }
            };
            var transactionDetail = new List<CreateWarehouseTransactionDetailDto>
            {
                new() { ProductId = productIds[0].Id, Quantity = 10, UnitPrice = 100  },
                new() { ProductId = productIds[1].Id, Quantity = 5, UnitPrice = 200  }
            };

            var warehouse = new Warehouse("Da Nang", "123 NVC", 1000, Guid.NewGuid());
            _unitOfWorkMock.Setup(uow => uow.Warehouses).Returns(_warehouseRepoMock.Object);
            _warehouseRepoMock.Setup(repo => repo.ExistsByIdAsync(warehouse.Id)).ReturnsAsync(true);


            _unitOfWorkMock.Setup(uow => uow.Products).Returns(_productRepoMock.Object);
            _productRepoMock.Setup(repo => repo.GetByIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(
                [
                    productIds[0],
                    productIds[1]
                ]);

            var user = new User("test@gmail.com", "hashpassword");

            _unitOfWorkMock.Setup(uow => uow.User).Returns(_userRepoMock.Object);
            _userRepoMock.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);

            var createtransaction = new CreateWarehouseTransactionDto { 
                TransactionType = TransactionTypes.Inbound,
                Status = "Pending",
                CreatedBy = user.Id,
                ReferenceNumber = "1",
                WarehouseId = warehouse.Id,
                TransactionDetails = transactionDetail };

            _unitOfWorkMock.Setup(x => x.WarehouseTransactions).Returns(_transactionRepoMock.Object);
            _transactionRepoMock.Setup(x => x.ExistsByReferenceNumberAsync(It.IsAny<string>())).ReturnsAsync(false);
            _mapperMock.Setup(x => x.Map<WarehouseTransactionDto>(It.IsAny<WarehouseTransaction>()))
                .Returns(new WarehouseTransactionDto {
                    Id = Guid.NewGuid(),
                    ReferenceNumber = "1",
                    TransactionType = TransactionTypes.Inbound,
                    Status = "Pending",
                    CreatedBy = user.Id,
                    WarehouseId = warehouse.Id });

            //Act
            var result = await _transactionservice.CreateTransactionAsync(createtransaction);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("1", result.ReferenceNumber);
            Assert.Equal(TransactionTypes.Inbound, result.TransactionType);
            Assert.Equal("Pending", result.Status);
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.NotEqual(Guid.Empty, result.CreatedBy);
            Assert.NotEqual(Guid.Empty, result.WarehouseId);

        }

        [Fact]
        public async Task CreateTransaction_ShouldThrowConflictException_WhenReferenceNumberExists()
        {
            //Arrange
            var transactionDetail = new List<CreateWarehouseTransactionDetailDto>
            {
                new() { ProductId = Guid.NewGuid(), Quantity = 10, UnitPrice = 100  },
                new() { ProductId = Guid.NewGuid(), Quantity = 5, UnitPrice = 200  }
            };
            var createtransaction = new CreateWarehouseTransactionDto { TransactionType = TransactionTypes.Inbound, Status = "Pending", CreatedBy = Guid.NewGuid(), ReferenceNumber = "1", WarehouseId = Guid.NewGuid(), TransactionDetails = transactionDetail };
            _unitOfWorkMock.Setup(x => x.WarehouseTransactions).Returns(_transactionRepoMock.Object);
            _transactionRepoMock.Setup(x => x.ExistsByReferenceNumberAsync(It.IsAny<string>())).ReturnsAsync(true);

            //Act & Assert
            await Assert.ThrowsAsync<ConflictException>(() => _transactionservice.CreateTransactionAsync(createtransaction));
        }

        [Fact]
        public async Task CreateTransaction_ShouldThrowKeyNotFoundException_WhenWarehouseNotFound()
        {
            //Arrange
            var transactionDetail = new List<CreateWarehouseTransactionDetailDto>
            {
                new() { ProductId = Guid.NewGuid(), Quantity = 10, UnitPrice = 100  },
                new() { ProductId = Guid.NewGuid(), Quantity = 5, UnitPrice = 200  }
            };
            var createtransaction = new CreateWarehouseTransactionDto { TransactionType = TransactionTypes.Inbound, Status = "Pending", CreatedBy = Guid.NewGuid(), ReferenceNumber = "1", WarehouseId = Guid.NewGuid(), TransactionDetails = transactionDetail };
            _unitOfWorkMock.Setup(x => x.WarehouseTransactions).Returns(_transactionRepoMock.Object);
            _transactionRepoMock.Setup(x => x.ExistsByReferenceNumberAsync(It.IsAny<string>())).ReturnsAsync(false);
            _unitOfWorkMock.Setup(x => x.Warehouses.ExistsByIdAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            //Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _transactionservice.CreateTransactionAsync(createtransaction));
        }

        [Fact]
        public async Task CreateTransaction_ShouldThrowKeyNotFoundException_WhenProductIdNotFound()
        {
            //Arrange
            var productIds = new List<Product>
            {
                new("Product1", "Description1", "SKU1", 1) { Name = "Product1", Description = "Description1" },
                new("Product2", "Description2", "SKU2", 1) { Name = "Product2", Description = "Description2" }
            };
            var transactionDetail = new List<CreateWarehouseTransactionDetailDto>
            {
                new() { ProductId = productIds[0].Id, Quantity = 10, UnitPrice = 100  },
                new() { ProductId = productIds[1].Id, Quantity = 5, UnitPrice = 200  }
            };

            var createtransaction = new CreateWarehouseTransactionDto { TransactionType = TransactionTypes.Inbound, Status = "Pending", CreatedBy = Guid.NewGuid(), ReferenceNumber = "1", WarehouseId = Guid.NewGuid(), TransactionDetails = transactionDetail };
            _unitOfWorkMock.Setup(x => x.WarehouseTransactions).Returns(_transactionRepoMock.Object);
            _transactionRepoMock.Setup(x => x.ExistsByReferenceNumberAsync(It.IsAny<string>())).ReturnsAsync(false);
            _unitOfWorkMock.Setup(x => x.Warehouses.ExistsByIdAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            _unitOfWorkMock.Setup(x => x.Products).Returns(_productRepoMock.Object);
            _productRepoMock.Setup(x => x.GetByIdsAsync(It.IsAny<List<Guid>>())).ReturnsAsync(productIds.Take(1).ToList());

            //Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _transactionservice.CreateTransactionAsync(createtransaction));
        }

        [Fact]
        public async Task CreateTransaction_ShouldThrowConflictException_WhenProductIsDuplicated()
        {
            //Arrange
            var productIds = new List<Product>
            {
                new("Product1", "Description1", "SKU1", 1) { Name = "Product1", Description = "Description1" },
                new("Product2", "Description2", "SKU2", 1) { Name = "Product2", Description = "Description2" }
            };
            var transactionDetail = new List<CreateWarehouseTransactionDetailDto>
            {
                new() { ProductId = productIds[0].Id, Quantity = 10, UnitPrice = 100  },
                new() { ProductId = productIds[1].Id, Quantity = 5, UnitPrice = 200  },
                new() { ProductId = productIds[0].Id, Quantity = 3, UnitPrice = 150  }
            };

            var createtransaction = new CreateWarehouseTransactionDto { TransactionType = TransactionTypes.Inbound, Status = "Pending", CreatedBy = Guid.NewGuid(), ReferenceNumber = "1", WarehouseId = Guid.NewGuid(), TransactionDetails = transactionDetail };
            _unitOfWorkMock.Setup(x => x.WarehouseTransactions).Returns(_transactionRepoMock.Object);
            _transactionRepoMock.Setup(x => x.ExistsByReferenceNumberAsync(It.IsAny<string>())).ReturnsAsync(false);
            _unitOfWorkMock.Setup(x => x.Warehouses.ExistsByIdAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            _unitOfWorkMock.Setup(x => x.Products).Returns(_productRepoMock.Object);
            _productRepoMock.Setup(x => x.GetByIdsAsync(It.IsAny<List<Guid>>())).ReturnsAsync(productIds.Take(2).ToList());

            //Act & Assert
            await Assert.ThrowsAsync<ConflictException>(() => _transactionservice.CreateTransactionAsync(createtransaction));
        }
        #endregion

        #region ApproveTransactionAsync Tests
        [Fact]
        public async Task ApproveTransaction_ShouldReturnSuccess_WhenDataValid()
        {
            //Arrange
            var transactionDetails = new List<WarehouseTransactionDetail>
            {
                new(Guid.NewGuid(), 10, 100, string.Empty),
                new(Guid.NewGuid(), 5, 200, string.Empty)
            };
            var transaction = new WarehouseTransaction(TransactionTypes.Inbound, Guid.NewGuid(), Guid.NewGuid(), "Pending", "1");

            _currentServiceMock.Setup(x => x.UserId).Returns(transaction.Id);
            _unitOfWorkMock.Setup(x => x.WarehouseTransactions).Returns(_transactionRepoMock.Object);
            _transactionRepoMock.Setup(x => x.GetByIdAsync(transaction.Id, t => t.TransactionDetails)).ReturnsAsync(transaction);
            _mapperMock.Setup(x => x.Map<WarehouseTransactionDto>(It.IsAny<WarehouseTransaction>()))
                .Returns((WarehouseTransaction wt) => new WarehouseTransactionDto
                {
                    Id = wt.Id,
                    ReferenceNumber = wt.ReferenceNumber,
                    TransactionType = wt.TransactionType,
                    Status = wt.Status,
                    CreatedBy = wt.CreatedBy,
                    WarehouseId = wt.WarehouseId
                });

            //Act
            var result = await _transactionservice.ApproveTransactionAsync(transaction.Id, _currentServiceMock.Object);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(transaction.Id, result.Id);
            Assert.Equal("Approved", result.Status);
        }

        [Fact]
        public async Task ApproveTransaction_ShouldReturnKeyNotFoundException_WhenTransactionIdNotFound()
        {
            //Arrange
            var transactionDetails = new List<WarehouseTransactionDetail>
            {
                new(Guid.NewGuid(), 10, 100, string.Empty),
                new(Guid.NewGuid(), 5, 200, string.Empty)
            };
            var transaction = new WarehouseTransaction(TransactionTypes.Inbound, Guid.NewGuid(), Guid.NewGuid(), "Pending", "1");

            _unitOfWorkMock.Setup(x => x.WarehouseTransactions).Returns(_transactionRepoMock.Object);
            _transactionRepoMock.Setup(x => x.GetByIdAsync(transaction.Id, t => t.TransactionDetails)).ReturnsAsync((WarehouseTransaction?)null);

            //Act&&Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _transactionservice.ApproveTransactionAsync(transaction.Id, _currentServiceMock.Object));
        }
        #endregion

        #region RejectTransactionAsync Tests
        [Fact]
        public async Task RejectTransaction_ShouldReturnSuccess_WhenDataValid()
        {
            //Arrange
            var transaction = new WarehouseTransaction(TransactionTypes.Inbound, Guid.NewGuid(), Guid.NewGuid(), "Pending", "1");
            _unitOfWorkMock.Setup(x => x.WarehouseTransactions).Returns(_transactionRepoMock.Object);
            _transactionRepoMock.Setup(x => x.GetByIdAsync(transaction.Id)).ReturnsAsync(transaction);
            _mapperMock.Setup(x => x.Map<WarehouseTransactionDto>(It.IsAny<WarehouseTransaction>()))
                .Returns((WarehouseTransaction wt) => new WarehouseTransactionDto
                {
                    Id = wt.Id,
                    ReferenceNumber = wt.ReferenceNumber,
                    TransactionType = wt.TransactionType,
                    Status = wt.Status,
                    CreatedBy = wt.CreatedBy,
                    WarehouseId = wt.WarehouseId
                });
            //Act
            var result = await _transactionservice.RejectTransactionAsync(transaction.Id, "Invalid data");

            //Assert
            Assert.NotNull(result);
            Assert.Equal(transaction.Id, result.Id);
            Assert.Equal("Rejected", result.Status);
        }

        [Fact]
        public async Task RejectTransaction_ShouldReturnKeyNotFoundException_WhenTransactionIdNotFound()
        {
            //Arrange
            var transaction = new WarehouseTransaction(TransactionTypes.Inbound, Guid.NewGuid(), Guid.NewGuid(), "Pending", "1");
            _unitOfWorkMock.Setup(x => x.WarehouseTransactions).Returns(_transactionRepoMock.Object);
            _transactionRepoMock.Setup(x => x.GetByIdAsync(transaction.Id)).ReturnsAsync((WarehouseTransaction?)null);

            //Act&&Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _transactionservice.RejectTransactionAsync(transaction.Id, "Invalid data"));
        }
        #endregion

        #region GetAllTransactionsAsync Tests
        [Fact]
        public async Task GetAllTransactions_ShouldReturnSuccess_WhenTransactionExist()
        {
            //Arrange
            var transactions = new List<WarehouseTransaction>
            {
                new(TransactionTypes.Inbound, Guid.NewGuid(), Guid.NewGuid(), "Approved", "1"),
                new(TransactionTypes.Outbound, Guid.NewGuid(), Guid.NewGuid(), "Rejected", "2")
            };
            _unitOfWorkMock.Setup(x => x.WarehouseTransactions).Returns(_transactionRepoMock.Object);
            _transactionRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(transactions);
            _mapperMock.Setup(x => x.Map<IEnumerable<WarehouseTransactionDto>>(It.IsAny<IEnumerable<WarehouseTransaction>>()))
                .Returns((IEnumerable<WarehouseTransaction> wts) => wts.Select(wt => new WarehouseTransactionDto
                {
                    Id = wt.Id,
                    ReferenceNumber = wt.ReferenceNumber,
                    TransactionType = wt.TransactionType,
                    Status = wt.Status,
                    CreatedBy = wt.CreatedBy,
                    WarehouseId = wt.WarehouseId
                }));

            //Act
            var result = await _transactionservice.GetAllTransactionsAsync();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllTransactions_ShouldReturnListEmpty_WhenNoTransactionExist()
        {
            //Arrange
            var transactions = new List<WarehouseTransaction>
            {
                new(TransactionTypes.Inbound, Guid.NewGuid(), Guid.NewGuid(), "Approved", "1"),
                new(TransactionTypes.Outbound, Guid.NewGuid(), Guid.NewGuid(), "Rejected", "2")
            };
            _unitOfWorkMock.Setup(x => x.WarehouseTransactions).Returns(_transactionRepoMock.Object);
            _transactionRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync([]);
            _mapperMock.Setup(x => x.Map<IEnumerable<WarehouseTransactionDto>>(It.IsAny<IEnumerable<WarehouseTransaction>>()))
                .Returns((IEnumerable<WarehouseTransaction> wts) => wts.Select(wt => new WarehouseTransactionDto
                {
                    Id = wt.Id,
                    ReferenceNumber = wt.ReferenceNumber,
                    TransactionType = wt.TransactionType,
                    Status = wt.Status,
                    CreatedBy = wt.CreatedBy,
                    WarehouseId = wt.WarehouseId
                }));

            //Act
            var result = await _transactionservice.GetAllTransactionsAsync();

            //Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
        #endregion

        #region GetTransactionByIdAsync Tests
        [Fact]
        public async Task GetTransactionById_ShouldReturnSuccess_WhenTransactionIdExist()
        {
            //Arrange
            var transaction = new WarehouseTransaction(TransactionTypes.Inbound, Guid.NewGuid(), Guid.NewGuid(), "Approved", "1");

            _unitOfWorkMock.Setup(x => x.WarehouseTransactions).Returns(_transactionRepoMock.Object);
            _transactionRepoMock.Setup(x => x.GetByIdAsync(transaction.Id)).ReturnsAsync(transaction);
            _mapperMock.Setup(x => x.Map<WarehouseTransactionDto>(It.IsAny<WarehouseTransaction>()))
                .Returns((WarehouseTransaction wt) => new WarehouseTransactionDto
                {
                    Id = wt.Id,
                    ReferenceNumber = wt.ReferenceNumber,
                    TransactionType = wt.TransactionType,
                    Status = wt.Status,
                    CreatedBy = wt.CreatedBy,
                    WarehouseId = wt.WarehouseId
                });

            //Act
            var result = await _transactionservice.GetTransactionByIdAsync(transaction.Id);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(transaction.Id, result.Id);
        }

        [Fact]
        public async Task GetTransactionById_ShouldReturnKeyNotFoundException_WhenTransactionIdNotExist()
        {
            var transaction = new WarehouseTransaction(TransactionTypes.Inbound, Guid.NewGuid(), Guid.NewGuid(), "Approved", "1");
            _unitOfWorkMock.Setup(x => x.WarehouseTransactions).Returns(_transactionRepoMock.Object);
            _transactionRepoMock.Setup(x => x.GetByIdAsync(transaction.Id)).ReturnsAsync((WarehouseTransaction?)null);

            //Act&&Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _transactionservice.GetTransactionByIdAsync(transaction.Id));
        }
        #endregion

        #region DeleteTransactionAsync Tests
        [Fact]
        public async Task DeleteTransaction_ShouldReturnSuccess_WhenTransactionIdExist()
        {
            //Arrange
            var transaction = new WarehouseTransaction(TransactionTypes.Inbound, Guid.NewGuid(), Guid.NewGuid(), "Approved", "1");
            _unitOfWorkMock.Setup(x => x.WarehouseTransactions).Returns(_transactionRepoMock.Object);
            _transactionRepoMock.Setup(x => x.GetByIdAsync(transaction.Id)).ReturnsAsync(transaction);

            //Act
            await _transactionservice.DeleteTransactionAsync(transaction.Id);

            //Assert
            _transactionRepoMock.Verify(t => t.Delete(transaction), Times.Once);
            _unitOfWorkMock.Verify(t => t.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteTransaction_ShouldReturnKeyNotFoundException_WhenTransactionIdNotExist()
        {
            //Arrange
            var transaction = new WarehouseTransaction(TransactionTypes.Inbound, Guid.NewGuid(), Guid.NewGuid(), "Approved", "1");
            _unitOfWorkMock.Setup(x => x.WarehouseTransactions).Returns(_transactionRepoMock.Object);
            _transactionRepoMock.Setup(x => x.GetByIdAsync(transaction.Id)).ReturnsAsync((WarehouseTransaction?)null);

            //Act&&Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _transactionservice.DeleteTransactionAsync(transaction.Id));
        }
        #endregion

        #region UpdateTransactionAsync Tests
        [Fact]
        public async Task UpdateTransaction_ShouldReturnSuccess_WhenDataValid()
        {
            //Arrange
            var transaction = new WarehouseTransaction(TransactionTypes.Inbound, Guid.NewGuid(), Guid.NewGuid(), "Pending", "1");
            var updateDto = new UpdateWarehouseTransactionDto
            {
                Status = "Approved",
                ReferenceNumber = "2",
                WarehouseId = Guid.NewGuid()
            };
            _unitOfWorkMock.Setup(x => x.WarehouseTransactions).Returns(_transactionRepoMock.Object);
            _transactionRepoMock.Setup(x => x.GetByIdAsync(transaction.Id)).ReturnsAsync(transaction);
            _mapperMock.Setup(x => x.Map<WarehouseTransactionDto>(It.IsAny<WarehouseTransaction>()))
                .Returns((WarehouseTransaction wt) => new WarehouseTransactionDto
                {
                    Id = wt.Id,
                    ReferenceNumber = wt.ReferenceNumber,
                    TransactionType = wt.TransactionType,
                    Status = wt.Status,
                    CreatedBy = wt.CreatedBy,
                    WarehouseId = wt.WarehouseId
                });
            //Act
            var result = await _transactionservice.UpdateTransactionAsync(transaction.Id, updateDto, _currentServiceMock.Object);
            //Assert
            Assert.NotNull(result);
            Assert.Equal(transaction.Id, result.Id);
            Assert.Equal("Approved", result.Status);
            Assert.Equal("2", result.ReferenceNumber);
            Assert.Equal(updateDto.WarehouseId, result.WarehouseId);
        }

        [Fact]
        public async Task UpdateTransaction_ShouldReturnKeyNotFoundException_WhenTransactionIdNotExist()
        {
            //Arrange
            var transaction = new WarehouseTransaction(TransactionTypes.Inbound, Guid.NewGuid(), Guid.NewGuid(), "Pending", "1");
            var updateDto = new UpdateWarehouseTransactionDto
            {
                Status = "Approved",
                ReferenceNumber = "2",
                WarehouseId = Guid.NewGuid()
            };
            _unitOfWorkMock.Setup(x => x.WarehouseTransactions).Returns(_transactionRepoMock.Object);
            _transactionRepoMock.Setup(x => x.GetByIdAsync(transaction.Id)).ReturnsAsync((WarehouseTransaction?)null);

            //Act&&Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _transactionservice.UpdateTransactionAsync(transaction.Id, updateDto, _currentServiceMock.Object));
        }

        [Fact]
        public async Task UpdateTransaction_ShouldReturnSuccess_WhenNoFieldsToUpdate()
        {
            //Arrange
            var transaction = new WarehouseTransaction(TransactionTypes.Inbound, Guid.NewGuid(), Guid.NewGuid(), "Pending", "1");
            var updateDto = new UpdateWarehouseTransactionDto { };
            _unitOfWorkMock.Setup(x => x.WarehouseTransactions).Returns(_transactionRepoMock.Object);
            _transactionRepoMock.Setup(x => x.GetByIdAsync(transaction.Id)).ReturnsAsync(transaction);
            _mapperMock.Setup(x => x.Map<WarehouseTransactionDto>(It.IsAny<WarehouseTransaction>()))
                .Returns((WarehouseTransaction wt) => new WarehouseTransactionDto
                {
                    Id = wt.Id,
                    ReferenceNumber = wt.ReferenceNumber,
                    TransactionType = wt.TransactionType,
                    Status = wt.Status,
                    CreatedBy = wt.CreatedBy,
                    WarehouseId = wt.WarehouseId
                });
            //Act
            var result = await _transactionservice.UpdateTransactionAsync(transaction.Id, updateDto, _currentServiceMock.Object);
            //Assert
            Assert.NotNull(result);
            Assert.Equal(transaction.Id, result.Id);
            Assert.Equal("Pending", result.Status);
            Assert.Equal("1", result.ReferenceNumber);
            Assert.Equal(transaction.WarehouseId, result.WarehouseId);
        }

        [Fact]
        public async Task UpdateTransaction_ShouldReturnSuccess_WhenOnlyStatusToUpdate()
        {
            //Arrange
            var transaction = new WarehouseTransaction(TransactionTypes.Inbound, Guid.NewGuid(), Guid.NewGuid(), "Pending", "1");
            var updateDto = new UpdateWarehouseTransactionDto { Status = "Rejected", RejectionReason = "Invalid data" };
            _unitOfWorkMock.Setup(x => x.WarehouseTransactions).Returns(_transactionRepoMock.Object);
            _transactionRepoMock.Setup(x => x.GetByIdAsync(transaction.Id)).ReturnsAsync(transaction);
            _mapperMock.Setup(x => x.Map<WarehouseTransactionDto>(It.IsAny<WarehouseTransaction>()))
                .Returns((WarehouseTransaction wt) => new WarehouseTransactionDto
                {
                    Id = wt.Id,
                    ReferenceNumber = wt.ReferenceNumber,
                    TransactionType = wt.TransactionType,
                    Status = wt.Status,
                    CreatedBy = wt.CreatedBy,
                    WarehouseId = wt.WarehouseId
                });

            //Act
            var result = await _transactionservice.UpdateTransactionAsync(transaction.Id, updateDto, _currentServiceMock.Object);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(transaction.Id, result.Id);
            Assert.Equal("Rejected", result.Status);
            Assert.Equal("1", result.ReferenceNumber);
            Assert.Equal(transaction.WarehouseId, result.WarehouseId);
        }

        [Fact]
        public async Task UpdateTransaction_ShouldReturnSuccess_WhenOnlyReferenceNumberToUpdate()
        {
            //Arrange
            var transaction = new WarehouseTransaction(TransactionTypes.Inbound, Guid.NewGuid(), Guid.NewGuid(), "Pending", "1");
            var updateDto = new UpdateWarehouseTransactionDto { ReferenceNumber = "2" };
            _unitOfWorkMock.Setup(x => x.WarehouseTransactions).Returns(_transactionRepoMock.Object);
            _transactionRepoMock.Setup(x => x.GetByIdAsync(transaction.Id)).ReturnsAsync(transaction);
            _mapperMock.Setup(x => x.Map<WarehouseTransactionDto>(It.IsAny<WarehouseTransaction>()))
                .Returns((WarehouseTransaction wt) => new WarehouseTransactionDto
                {
                    Id = wt.Id,
                    ReferenceNumber = wt.ReferenceNumber,
                    TransactionType = wt.TransactionType,
                    Status = wt.Status,
                    CreatedBy = wt.CreatedBy,
                    WarehouseId = wt.WarehouseId
                });
            //Act
            var result = await _transactionservice.UpdateTransactionAsync(transaction.Id, updateDto, _currentServiceMock.Object);
            //Assert
            Assert.NotNull(result);
            Assert.Equal(transaction.Id, result.Id);
            Assert.Equal("Pending", result.Status);
            Assert.Equal("2", result.ReferenceNumber);
            Assert.Equal(transaction.WarehouseId, result.WarehouseId);
        }
        #endregion
    }
}
