using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Domain.Interfaces;
using WarehouseManagement.Infrastructure.Data;

namespace WarehouseManagement.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WarehouseDbContext _context;

        public UnitOfWork(WarehouseDbContext context)
        {
            _context = context;
            Products = new ProductRepository(_context);
            Warehouses = new WarehouseRepository(_context);
            Stocks = new StockRepository(_context);
            WarehouseTransactions = new WarehouseTransactionRepository(_context);
            User = new UserRepository(_context);
            Supplier = new SupplierRepository(_context);
            Categories = new CategoryRepository(_context);
        }

        public IProductRepository Products { get; }
        public IWarehouseRepository Warehouses { get; }
        public IStockRepository Stocks { get; }
        public IWarehouseTransactionRepository WarehouseTransactions { get; }
        public IUserRepository User { get; }
        public ISupplierRepository Supplier { get; }
        public ICategoryRepository Categories { get; }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        //public void Dispose() => _context.Dispose();
    }
}
