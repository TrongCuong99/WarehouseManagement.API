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
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(20);
            builder.Property(u => u.Password).IsRequired().HasMaxLength(255);
            builder.Property(u => u.Role).IsRequired().HasMaxLength(10);
            builder.Property(u => u.CreatedAt).IsRequired();
            builder.HasMany(u => u.Warehouses)
                   .WithOne(w => w.User)
                   .HasForeignKey(w => w.UserId);
            builder.HasMany(u => u.CreatedTransactions)
                   .WithOne(t => t.CreatedByUser)
                   .HasForeignKey(t => t.CreatedBy);
            builder.HasMany(u => u.ApprovedTransactions)
                   .WithOne(t => t.ApprovedByUser)
                   .HasForeignKey(t => t.ApprovedBy)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
