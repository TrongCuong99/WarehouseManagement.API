using Microsoft.EntityFrameworkCore;
using System.Transactions;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Domain.Enums;
using WarehouseManagement.Domain.Interfaces;
using WarehouseManagement.Infrastructure.Data;

namespace WarehouseManagement.Infrastructure.Repositories
{
    public class WarehouseTransactionRepository(WarehouseDbContext context) :
        BaseRepository<WarehouseTransaction>(context), IWarehouseTransactionRepository
    {
        public async Task ApproveTransactionAsync(Guid transactionId, Guid approvedByUserId)
        {
            var transaction = await _dbSet.FindAsync(transactionId) ?? throw new KeyNotFoundException("Transaction not found.");
            transaction.Approve(approvedByUserId);
        }

        public async Task<bool> ExistsByReferenceNumberAsync(string referenceNumber)
        {
            return await _dbSet.AnyAsync(t => t.ReferenceNumber == referenceNumber);
        }

        public async Task<IEnumerable<WarehouseTransaction>> GetByDateRangeAsync(DateTime start, DateTime end)
        {
            return await _dbSet
                .Where(t => t.TransactionDate >= start && t.TransactionDate <= end)
                .ToListAsync();
        }

        public async Task<WarehouseTransaction?> GetByReferenceNumberAsync(string referenceNumber)
        {
            return await _dbSet.FirstOrDefaultAsync(t => t.ReferenceNumber == referenceNumber);
        }

        public async Task<IEnumerable<WarehouseTransaction>> GetByUserAsync(Guid userId)
        {
            return await _dbSet
                .Where(t => t.CreatedBy == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<WarehouseTransaction>> GetByWarehouseAsync(Guid warehouseId)
        {
            return await _dbSet
                .Where(t => t.WarehouseId == warehouseId)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalExportValueAsync(Guid warehouseId, DateTime from, DateTime to)
        {
            return await _dbSet
                .Where(t => t.WarehouseId == warehouseId
                            && t.TransactionType == TransactionTypes.Outbound
                            && t.TransactionDate >= from
                            && t.TransactionDate <= to)
                .SelectMany(t => t.TransactionDetails)
                .SumAsync(d => d.Quantity * d.UnitPrice);
        }

        public async Task<decimal> GetTotalImportValueAsync(Guid warehouseId, DateTime from, DateTime to)
        {
            return await _dbSet
                .Where(t => t.WarehouseId == warehouseId
                            && t.TransactionType == TransactionTypes.Inbound
                            && t.TransactionDate >= from
                            && t.TransactionDate <= to)
                .SelectMany(t => t.TransactionDetails)
                .SumAsync(d => d.Quantity * d.UnitPrice);
        }

        public async Task<WarehouseTransaction?> GetTransactionWithDetailsAsync(Guid transactionId)
        {
            return await _dbSet
                .Include(t => t.TransactionDetails)
                .FirstOrDefaultAsync(t => t.Id == transactionId);
        }

        public async Task RejectTransactionAsync(Guid transactionId, string reason)
        {
            var transaction = await _dbSet.FindAsync(transactionId) ?? throw new KeyNotFoundException("Transaction not found.");
            transaction.Rejected(reason);
        }
    }
}
