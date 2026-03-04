using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Infrastructure.Data.Configurations
{
    public class StockConfiguration : IEntityTypeConfiguration<Stock>
    {
        public void Configure(EntityTypeBuilder<Stock> builder)
        {
            //builder.HasIndex(s => s.QuantityOnHand);
            builder.HasKey(s => s.Id);
            builder.Property(s => s.ProductId).IsRequired();
            builder.Property(s => s.WarehouseId).IsRequired();
            builder.Property(s => s.QuantityOnHand).IsRequired();
            builder.Property(s => s.LastUpdated).IsRequired();
            builder.Property(s => s.ReorderLevel).IsRequired();
            builder.HasOne(s => s.Product)
                   .WithMany(p => p.Stocks)
                   .HasForeignKey(s => s.ProductId);

            builder.HasOne(s => s.Warehouse)
                   .WithMany(w => w.Stocks)
                   .HasForeignKey(s => s.WarehouseId);
        }
    }
}
