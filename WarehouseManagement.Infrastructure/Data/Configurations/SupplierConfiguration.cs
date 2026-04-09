using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Infrastructure.Data.Configurations
{
    public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
            builder.Property(s => s.ContactEmail).HasMaxLength(100);
            builder.Property(s => s.PhoneNumber).HasMaxLength(15);
            builder.Property(s => s.Address).HasMaxLength(300);
            builder.Property(s => s.CreatedAt).IsRequired();
            builder.HasMany(s => s.ProductSuppliers)
                   .WithOne(p => p.Supplier)
                   .HasForeignKey(ps => ps.SupplierId);
        }
    }
}
