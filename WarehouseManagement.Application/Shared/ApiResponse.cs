using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Application.Shared
{
    public class ApiResponse<T>(int status, string message, T? data = default)
    {
        public int Status { get; set; } = status;
        public string Message { get; set; } = message;
        public T? Data { get; set; } = data;
    }
}
