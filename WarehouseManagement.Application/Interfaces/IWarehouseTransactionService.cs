using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Application.Comom;
using WarehouseManagement.Application.DTOs.WarehouseTransactions;

namespace WarehouseManagement.Application.Interfaces
{
    public interface IWarehouseTransactionService
    {
        Task<WarehouseTransactionDto> CreateTransactionAsync(CreateWarehouseTransactionDto dto);
        Task<WarehouseTransactionDto> ApproveTransactionAsync(int transactionId, ICurrentUserService currentUserService);
        Task<WarehouseTransactionDto> RejectTransactionAsync(int transactionId, string reason);
        Task<IQueryable<WarehouseTransactionDto>> GetAllTransactionsAsync();
        Task<WarehouseTransactionDto> GetTransactionByIdAsync(int transactionId);
        Task<WarehouseTransactionDto> UpdateTransactionAsync(int transactionId, UpdateWarehouseTransactionDto dto, ICurrentUserService currentUserService);
        Task DeleteTransactionAsync(int transactionId);
    }
}
