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
    public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
    {
        public void Configure(EntityTypeBuilder<Warehouse> builder)
        {
            builder.HasKey(w => w.Id);
            builder.Property(w => w.Name).IsRequired();
            builder.Property(w => w.Location).IsRequired().HasMaxLength(200);
            builder.Property(w => w.Capacity).IsRequired();
            builder.Property(w => w.CreatedAt).IsRequired();
            builder.HasOne(w => w.User)
                   .WithMany(s => s.Warehouses)
                   .HasForeignKey(s => s.UserId);
        }
    }
}
