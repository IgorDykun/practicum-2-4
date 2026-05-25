using Microsoft.EntityFrameworkCore;
using Nimble.Modulith.Customers.Domain.CustomerAggregate;

namespace Nimble.Modulith.Customers.Infrastructure.Data;

public class CustomersDbContext : DbContext
{
    public CustomersDbContext(DbContextOptions<CustomersDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("Customers");
        builder.ApplyConfigurationsFromAssembly(typeof(CustomersDbContext).Assembly);
    }
}