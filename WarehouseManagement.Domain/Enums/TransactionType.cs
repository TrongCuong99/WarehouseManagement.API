using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Domain.Enums
{
    public enum TransactionTypes
    {
        Inbound = 1,
        Outbound = 2,
        Adjustment = 3,
        Pending = 4,
        Approved = 5,
        Rejected = 6
    }
}
