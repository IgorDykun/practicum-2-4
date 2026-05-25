using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nimble.Modulith.Reporting.Models;

namespace Nimble.Modulith.Reporting.Data.Config;

public class FactOrderConfig : IEntityTypeConfiguration<FactOrder>
{
    public void Configure(EntityTypeBuilder<FactOrder> builder)
    {
        builder.ToTable("FactOrders");
        builder.HasKey(f => new { f.OrderId, f.OrderItemId });

        builder.HasOne(f => f.DimDate).WithMany().HasForeignKey(f => f.DateKey);
        builder.HasOne(f => f.DimCustomer).WithMany().HasForeignKey(f => f.CustomerId);
        builder.HasOne(f => f.DimProduct).WithMany().HasForeignKey(f => f.ProductId);

        builder.Property(f => f.UnitPrice).HasColumnType("decimal(18,2)");
        builder.Property(f => f.TotalPrice).HasColumnType("decimal(18,2)");
        builder.Property(f => f.OrderTotalAmount).HasColumnType("decimal(18,2)");
    }
}