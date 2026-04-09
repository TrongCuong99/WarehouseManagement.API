using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Domain.Interfaces;

namespace WarehouseManagement.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IProductRepository Products { get; }
        IWarehouseRepository Warehouses { get; }
        IStockRepository Stocks { get; }
        IWarehouseTransactionRepository WarehouseTransactions { get; }
        IUserRepository User { get; }
        ISupplierRepository Supplier { get; }
        ICategoryRepository Categories { get; }
        Task<int> SaveChangesAsync();
    }
}
