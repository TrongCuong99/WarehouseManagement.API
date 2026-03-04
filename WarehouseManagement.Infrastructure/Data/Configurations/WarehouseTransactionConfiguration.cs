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
    public class WarehouseTransactionConfiguration : IEntityTypeConfiguration<WarehouseTransaction>
    {
        public void Configure(EntityTypeBuilder<WarehouseTransaction> builder)
        {
            //builder.HasIndex(t => t.TransactionDate);
            builder.HasKey(wt => wt.Id);
            builder.Property(wt => wt.TransactionType).IsRequired().HasMaxLength(50);
            builder.Property(wt => wt.TransactionDate).IsRequired();
            builder.Property(wt => wt.ReferenceNumber).HasMaxLength(100);
            builder.Property(wt => wt.WarehouseId).IsRequired().HasMaxLength(50);
            builder.Property(wt => wt.CreatedBy).IsRequired();
            builder.Property(wt => wt.CreatedAt).IsRequired();
            builder.Property(wt => wt.UpdateAt).IsRequired();
            builder.HasMany(wt => wt.TransactionDetails)
                   .WithOne(td => td.WarehouseTransaction)
                   .HasForeignKey(td => td.Id);

            builder.HasOne(t => t.CreatedByUser)
                   .WithMany(u => u.CreatedTransactions)
                   .HasForeignKey(t => t.CreatedBy)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.ApprovedByUser)
                   .WithMany(u => u.ApprovedTransactions)
                   .HasForeignKey(t => t.ApprovedBy)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
