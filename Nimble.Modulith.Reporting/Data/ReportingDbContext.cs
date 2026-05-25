using Microsoft.EntityFrameworkCore;
using Nimble.Modulith.Reporting.Models;

namespace Nimble.Modulith.Reporting.Data;

public class ReportingDbContext : DbContext
{
    public ReportingDbContext(DbContextOptions<ReportingDbContext> options) : base(options) { }

    public DbSet<FactOrder> FactOrders => Set<FactOrder>();
    public DbSet<DimDate> DimDates => Set<DimDate>();
    public DbSet<DimCustomer> DimCustomers => Set<DimCustomer>();
    public DbSet<DimProduct> DimProducts => Set<DimProduct>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReportingDbContext).Assembly);
    }
}