using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Infrastructure.Data.Configurations
{
    public class ProductSupplierConfiguration : IEntityTypeConfiguration<ProductSupplier>
    {
        public void Configure(EntityTypeBuilder<ProductSupplier> builder)
        {
            builder.HasKey(ps => ps.Id);
            builder.Property(builder => builder.SupplyPrice).HasPrecision(18, 2).IsRequired();
            builder.Property(builder => builder.ProductId).IsRequired();
            builder.Property(builder => builder.SupplierId).IsRequired();
            builder.HasKey(ps => new { ps.ProductId, ps.SupplierId });

            builder.HasOne(ps => ps.Product)
                   .WithMany(p => p.ProductSuppliers)
                   .HasForeignKey(ps => ps.ProductId);

            builder.HasOne(ps => ps.Supplier)
                   .WithMany(s => s.ProductSuppliers)
                   .HasForeignKey(ps => ps.SupplierId);
        }
    }
}
