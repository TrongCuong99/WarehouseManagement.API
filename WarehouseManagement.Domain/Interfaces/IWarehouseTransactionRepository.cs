using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Domain.Interfaces
{
    public interface IWarehouseTransactionRepository : IRepository<WarehouseTransaction>
    {
        Task<WarehouseTransaction?> GetTransactionWithDetailsAsync(Guid transactionId);
        Task<WarehouseTransaction?> GetByReferenceNumberAsync(string referenceNumber);
        Task<bool> ExistsByReferenceNumberAsync(string referenceNumber);
        Task<IEnumerable<WarehouseTransaction>> GetByWarehouseAsync(Guid warehouseId);
        Task<IEnumerable<WarehouseTransaction>> GetByUserAsync(Guid userId);
        Task<IEnumerable<WarehouseTransaction>> GetByDateRangeAsync(DateTime start, DateTime end);
        Task ApproveTransactionAsync(Guid transactionId, Guid approvedByUserId);
        Task RejectTransactionAsync(Guid transactionId, string reason);
        Task<decimal> GetTotalImportValueAsync(Guid warehouseId, DateTime from, DateTime to);
        Task<decimal> GetTotalExportValueAsync(Guid warehouseId, DateTime from, DateTime to);
    }
}
