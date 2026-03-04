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
    public class WarehouseTransactionDetailConfiguration : IEntityTypeConfiguration<WarehouseTransactionDetail>
    {
        public void Configure(EntityTypeBuilder<WarehouseTransactionDetail> builder)
        {
            builder.HasKey(wtd => wtd.Id);
            builder.Property(wtd => wtd.Quantity).IsRequired();
            builder.Property(wtd => wtd.UnitPrice).HasPrecision(18, 2).IsRequired();
            builder.Property(wtd => wtd.Remarks).HasMaxLength(200);
            builder.HasOne(wtd => wtd.WarehouseTransaction)
                   .WithMany(wt => wt.TransactionDetails)
                   .HasForeignKey(wtd => wtd.WarehouseTransactionId);
        }
    }
}
