using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Infrastructure.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            //builder.HasIndex(p => p.SKU).IsUnique();
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Description).IsRequired().HasMaxLength(500);
            builder.Property(p => p.Price).HasPrecision(18, 2);
            builder.Property(p => p.CreatedAt).IsRequired();
            builder.HasMany(p => p.Categories)
                   .WithMany(c => c.Products);
            builder.HasMany(p => p.ProductSuppliers)
                   .WithOne(ps => ps.Product)
                   .HasForeignKey(ps => ps.ProductId);
            builder.HasMany(p => p.TransactionDetails)
                   .WithOne(td => td.Product)
                   .HasForeignKey(td => td.ProductId);
        }
    }
}
