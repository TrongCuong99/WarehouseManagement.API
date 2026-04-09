using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Domain.Interfaces
{
    public interface IAuditLogger
    {
        Task LogActivity(string userId, string action);

        Task LogChangeAsync(string userId, string tableName, string entityId, object oldValues, object newValues);

        Task LogActivityAsync(string userId, string action, string detail);
    }
}
