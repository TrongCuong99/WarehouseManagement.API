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
        Task<WarehouseTransactionDto> ApproveTransactionAsync(Guid transactionId, ICurrentUserService currentUserService);
        Task<WarehouseTransactionDto> RejectTransactionAsync(Guid transactionId, string reason);
        Task<IEnumerable<WarehouseTransactionDto>> GetAllTransactionsAsync();
        Task<WarehouseTransactionDto> GetTransactionByIdAsync(Guid transactionId);
        Task<WarehouseTransactionDto> UpdateTransactionAsync(Guid transactionId, UpdateWarehouseTransactionDto dto, ICurrentUserService currentUserService);
        Task DeleteTransactionAsync(Guid transactionId);
    }
}
