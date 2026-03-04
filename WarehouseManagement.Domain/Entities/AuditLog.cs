using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Domain.Entities
{
    public class AuditLog
    {
        public Guid AuditLogId { get; set; }
        public string Action { get; set; } = string.Empty;
        public DateTime DataBefore { get; set; }
        public DateTime DataAfter { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public User User { get; set; } = null!;
    }
}
