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
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(e => e.OldValues).HasColumnType("nvarchar(max)");
            builder.Property(e => e.NewValues).HasColumnType("nvarchar(max)");
            builder.Property(e => e.Action).HasMaxLength(50);
            builder.Property(e => e.TableName).HasMaxLength(100);
            builder.Property(e => e.UserId).HasMaxLength(100);

        }
    }
}
