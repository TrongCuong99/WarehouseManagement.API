using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Domain.Interfaces;

namespace WarehouseManagement.Infrastructure.Data.Logging
{
    public class EfAuditLogger(WarehouseDbContext context) : IAuditLogger
    {
        private readonly WarehouseDbContext _context = context;

        public async Task LogActivity(string userId, string action)
        {
            var auditEntry = new AuditLog
            {
                UserId = userId,
                Action = action,
                Timestamp = DateTime.UtcNow
            };

            _context.Set<AuditLog>().Add(auditEntry);
            await _context.SaveChangesAsync();
        }

        public Task LogActivityAsync(string userId, string action, string detail)
        {
            throw new NotImplementedException();
        }

        public async Task LogChangeAsync(string userId, string tableName, string entityId, object oldValues, object newValues)
        {
            var auditEntry = new AuditLog
            {
                UserId = userId,
                Action = "UPDATE",
                TableName = tableName,
                EntityId = entityId,
                OldValues = JsonSerializer.Serialize(oldValues),
                NewValues = JsonSerializer.Serialize(newValues),
                Timestamp = DateTime.UtcNow
            };

            _context.Set<AuditLog>().Add(auditEntry);
            await _context.SaveChangesAsync();
        }
    }
}
