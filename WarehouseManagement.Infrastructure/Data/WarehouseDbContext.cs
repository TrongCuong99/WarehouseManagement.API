using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Infrastructure.Data
{
    public class WarehouseDbContext(DbContextOptions<WarehouseDbContext> options, IHttpContextAccessor httpContextAccessor) : DbContext(options)
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

        private string GetCurrentUserId()
        {

            return httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WarehouseDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();

            var entries = ChangeTracker.Entries()
                        .Where(e => e.Entity.GetType().Name != nameof(AuditLog) &&
                                   (e.State == EntityState.Added ||
                                    e.State == EntityState.Modified ||
                                    e.State == EntityState.Deleted))
                        .ToList();

            foreach (var entry in entries)
            {
                var oldValues = new Dictionary<string, object>();
                var newValues = new Dictionary<string, object>();

                foreach (var property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            newValues[propertyName] = property.CurrentValue!;
                            break;

                        case EntityState.Deleted:
                            oldValues[propertyName] = property.OriginalValue!;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                oldValues[propertyName] = property.OriginalValue!;
                                newValues[propertyName] = property.CurrentValue!;
                            }
                            break;
                    }
                }

                var auditLog = new AuditLog
                {
                    UserId = userId,
                    TableName = entry.Metadata.GetTableName(),
                    Action = entry.State.ToString(),
                    Timestamp = DateTime.UtcNow,

                    EntityId = entry.Properties
                        .FirstOrDefault(p => p.Metadata.IsPrimaryKey())
                        ?.CurrentValue?.ToString(),

                    OldValues = oldValues.Count > 0 ? JsonSerializer.Serialize(oldValues) : null,
                    NewValues = newValues.Count > 0 ? JsonSerializer.Serialize(newValues) : null
                };

                AuditLogs?.Add(auditLog);
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
