using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Domain.Common
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message)
        {
        }

        public DomainException(string message, Exception? innerException) : base(message, innerException)
        {
        }

        public class ConflictException(string message) : Exception(message)
        {
        }
    }


}
