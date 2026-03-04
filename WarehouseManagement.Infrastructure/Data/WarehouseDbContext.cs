using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Infrastructure.Data
{
    public class WarehouseDbContext(DbContextOptions<WarehouseDbContext> options) : DbContext(options)
    {
        public DbSet<User>? Users { get; set; }
        public DbSet<Product>? Products { get; set; }
        public DbSet<Warehouse>? Warehouses { get; set; }
        public DbSet<WarehouseTransaction>? WarehouseTransactions { get; set; }
        public DbSet<AuditLog>? AuditLogs { get; set; }
        public DbSet<Stock>? Stocks { get; set; }
        public DbSet<WarehouseTransactionDetail>? WarehouseTransactionDetails { get; set; }
        public DbSet<ProductSupplier>? ProductSuppliers { get; set; }
        public DbSet<Supplier>? Suppliers { get; set; }
        public DbSet<Category>? Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WarehouseDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
