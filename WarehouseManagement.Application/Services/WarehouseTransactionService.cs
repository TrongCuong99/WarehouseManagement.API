using AutoMapper;
using WarehouseManagement.Application.Comom;
using WarehouseManagement.Application.DTOs.WarehouseTransactions;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Domain.Enums;
using static WarehouseManagement.Domain.Common.DomainException;

namespace WarehouseManagement.Application.Services
{
    public class WarehouseTransactionService(IUnitOfWork unitOfWork, IStockService stockService, IMapper mapper) : IWarehouseTransactionService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IStockService _stockService = stockService;
        private readonly IMapper _mapper = mapper;

        public async Task<WarehouseTransactionDto> CreateTransactionAsync(CreateWarehouseTransactionDto dto)
        {
            var refnumberExist = await _unitOfWork.WarehouseTransactions.ExistsByReferenceNumberAsync(dto.ReferenceNumber);
            if(refnumberExist)
            {
                throw new ConflictException("ReferenNumber already exists");
            }

            var warehouseExists = await _unitOfWork.Warehouses.ExistsByIdAsync(dto.WarehouseId);
            if (!warehouseExists)
            {
                throw new KeyNotFoundException("Warehouse not found");
            }

            var productIds = dto.TransactionDetails.Select(td => td.ProductId).Distinct().ToList();
            var allProductsExist = await _unitOfWork.Products.GetByIdsAsync(productIds);

            if (allProductsExist.Count != productIds.Count)
                throw new KeyNotFoundException("One or more products not found");

            if (productIds.Count != dto.TransactionDetails.Count)
                throw new ConflictException("Duplicate product in transaction details");

            var userIdExist = await _unitOfWork.User.GetByIdAsync(dto.CreatedBy) ?? throw new KeyNotFoundException("UserId not Exist");
            var transaction = new WarehouseTransaction(dto.TransactionType, dto.WarehouseId, dto.CreatedBy, dto.Status, dto.ReferenceNumber);

            foreach (var item in dto.TransactionDetails)
            {
                var detail = new WarehouseTransactionDetail(
                        productId: item.ProductId,
                        quantity: item.Quantity,
                        unitPrice: item.UnitPrice,
                        remarks: string.Empty
                        );
                transaction.AddDetail(detail);
            }

            await _unitOfWork.WarehouseTransactions.AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<WarehouseTransactionDto>(transaction);
        }

        public async Task<WarehouseTransactionDto> ApproveTransactionAsync(Guid transactionId, ICurrentUserService currentUserService)
        {
            var transaction = await _unitOfWork.WarehouseTransactions.GetByIdAsync(transactionId, t => t.TransactionDetails)
                ?? throw new KeyNotFoundException("Transaction not found");

            transaction.Approve(currentUserService.UserId);

            foreach (var detail in transaction.TransactionDetails)
            {
                int change = transaction.TransactionType == TransactionTypes.Inbound
                    ? detail.Quantity
                    : -detail.Quantity;

                await _stockService.UpdateStockAsync(detail.ProductId, transaction.WarehouseId, change);
            }

            _unitOfWork.WarehouseTransactions.Update(transaction);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<WarehouseTransactionDto>(transaction);
        }

        public async Task<WarehouseTransactionDto> RejectTransactionAsync(Guid transactionId, string reason)
        {
            var transaction = await _unitOfWork.WarehouseTransactions.GetByIdAsync(transactionId)
                ?? throw new KeyNotFoundException("Transaction not found");

            transaction.Rejected(reason);
            _unitOfWork.WarehouseTransactions.Update(transaction);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<WarehouseTransactionDto>(transaction);
        }

        public async Task<IEnumerable<WarehouseTransactionDto>> GetAllTransactionsAsync()
        {
            var list = await _unitOfWork.WarehouseTransactions.GetAllAsync();
            return _mapper.Map<IEnumerable<WarehouseTransactionDto>>(list);
        }

        public async Task<WarehouseTransactionDto> GetTransactionByIdAsync(Guid transactionId)
        {
            var transaction = await _unitOfWork.WarehouseTransactions.GetByIdAsync(transactionId)
                ?? throw new KeyNotFoundException("Transaction not found");
            return _mapper.Map<WarehouseTransactionDto>(transaction);
        }

        public async Task DeleteTransactionAsync(Guid transactionId)
        {
            var transaction = await _unitOfWork.WarehouseTransactions.GetByIdAsync(transactionId)
                ?? throw new KeyNotFoundException("Transaction not found");
            _unitOfWork.WarehouseTransactions.Delete(transaction);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<WarehouseTransactionDto> UpdateTransactionAsync(Guid id, UpdateWarehouseTransactionDto dto, ICurrentUserService currentUserService)
        {
            var warehouseTransaction = await _unitOfWork.WarehouseTransactions.GetByIdAsync(id) ?? throw new KeyNotFoundException("Warehouse Transaction not found.");
            if (!string.IsNullOrWhiteSpace(dto.Status))
            {
                if(dto.Status == "Approved")
                {
                    warehouseTransaction.Approve(currentUserService.UserId);
                }
                else if (dto.Status == "Rejected")
                {
                    warehouseTransaction.Rejected(dto.RejectionReason!);
                }
                else
                {
                    warehouseTransaction.Pending();
                }    
            }
            if (!string.IsNullOrWhiteSpace(dto.ReferenceNumber))
            {
                warehouseTransaction.ReferenceNumber = dto.ReferenceNumber;
            }
            if (dto.WarehouseId != null)
            {
                warehouseTransaction.WarehouseId = (Guid)dto.WarehouseId!;
            }
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<WarehouseTransactionDto>(warehouseTransaction);
        }
    }
}
