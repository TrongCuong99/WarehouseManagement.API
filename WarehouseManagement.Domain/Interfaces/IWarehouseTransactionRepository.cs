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
        Task<WarehouseTransaction?> GetTransactionWithDetailsAsync(int transactionId);
        Task<WarehouseTransaction?> GetByReferenceNumberAsync(string referenceNumber);
        Task<bool> ExistsByReferenceNumberAsync(string referenceNumber);
        Task<IEnumerable<WarehouseTransaction>> GetByWarehouseAsync(int warehouseId);
        Task<IEnumerable<WarehouseTransaction>> GetByUserAsync(int userId);
        Task<IEnumerable<WarehouseTransaction>> GetByDateRangeAsync(DateTime start, DateTime end);
        Task ApproveTransactionAsync(int transactionId, int approvedByUserId);
        Task RejectTransactionAsync(int transactionId, string reason);
        Task<decimal> GetTotalImportValueAsync(int warehouseId, DateTime from, DateTime to);
        Task<decimal> GetTotalExportValueAsync(int warehouseId, DateTime from, DateTime to);
    }
}
