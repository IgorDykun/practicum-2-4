using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nimble.Modulith.Reporting.Data;
using Nimble.Modulith.Reporting.Models;
using Nimble.Modulith.Customers.Contracts;

namespace Nimble.Modulith.Reporting.Ingest;

public class OrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>
{
    private readonly ReportingDbContext _dbContext;
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public OrderCreatedEventHandler(ReportingDbContext dbContext, ILogger<OrderCreatedEventHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async ValueTask Handle(OrderCreatedEvent notification, CancellationToken ct)
    {
        _logger.LogInformation("Ingesting confirmed order {OrderId} into Star Schema...", notification.OrderId);

        using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
        try
        {
            var customerExists = await _dbContext.DimCustomers.AnyAsync(c => c.CustomerId == notification.CustomerId, ct);
            if (!customerExists)
            {
                _dbContext.DimCustomers.Add(new DimCustomer
                {
                    CustomerId = notification.CustomerId,
                    FirstName = "Customer", 
                    LastName = notification.CustomerId.ToString().Substring(0, 5),
                    Email = "imported@example.com"
                });
                await _dbContext.SaveChangesAsync(ct);
            }

            int dateKey = ConvertToDateKey(DateOnly.FromDateTime(notification.OrderDate));

            var factExists = await _dbContext.FactOrders.AnyAsync(f => f.OrderId == notification.OrderId, ct);
            if (!factExists)
            {
                _dbContext.FactOrders.Add(new FactOrder
                {
                    OrderId = notification.OrderId,
                    OrderItemId = Guid.NewGuid(), // генеруємо сурогатний ID для рядка факту
                    DateKey = dateKey,
                    CustomerId = notification.CustomerId,
                    ProductId = 1, // Дефолтний ID або зв'язок 
                    Quantity = 1,
                    UnitPrice = notification.TotalAmount,
                    TotalPrice = notification.TotalAmount,
                    OrderTotalAmount = notification.TotalAmount
                });
            }

            await _dbContext.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            _logger.LogInformation("Successfully ingested order {OrderId}", notification.OrderId);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            _logger.LogError(ex, "Error occurred during ingestion of order {OrderId}", notification.OrderId);
            throw;
        }
    }

    private static int ConvertToDateKey(DateOnly date)
    {
        return date.Year * 10000 + date.Month * 100 + date.Day;
    }
}