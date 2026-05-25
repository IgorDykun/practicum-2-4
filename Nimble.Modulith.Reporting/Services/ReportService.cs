using Dapper;
using Microsoft.EntityFrameworkCore;
using Nimble.Modulith.Reporting.Data;
using System.Data;

namespace Nimble.Modulith.Reporting.Services;

public class ReportService : IReportService
{
    private readonly ReportingDbContext _dbContext;

    public ReportService(ReportingDbContext dbContext) => _dbContext = dbContext;

    private IDbConnection GetConnection() => _dbContext.Database.GetDbConnection();

    public async Task<OrdersReportSummary> GetOrdersReportAsync(DateTime startDate, DateTime endDate)
    {
        int startKey = startDate.Year * 10000 + startDate.Month * 100 + startDate.Day;
        int endKey = endDate.Year * 10000 + endDate.Month * 100 + endDate.Day;

        const string sql = @"
            SELECT 
                f.OrderId,
                MAX(d.Date) as OrderDate,
                MAX(c.FirstName + ' ' + c.LastName) as CustomerName,
                COUNT(DISTINCT f.ProductId) as UniqueProductsCount,
                MAX(f.OrderTotalAmount) as TotalAmount
            FROM FactOrders f
            JOIN DimDate d ON f.DateKey = d.DateKey
            JOIN DimCustomer c ON f.CustomerId = c.CustomerId
            WHERE f.DateKey BETWEEN @startKey AND @endKey
            GROUP BY f.OrderId
            ORDER BY OrderDate DESC";

        using var conn = GetConnection();
        var rows = (await conn.QueryAsync<OrdersReportRow>(sql, new { startKey, endKey })).ToList();

        if (!rows.Any()) return new OrdersReportSummary();

        return new OrdersReportSummary
        {
            TotalOrders = rows.Count,
            TotalRevenue = rows.Sum(r => r.TotalAmount),
            AverageOrderValue = rows.Average(r => r.TotalAmount),
            Rows = rows
        };
    }

    public async Task<IEnumerable<ProductSalesReportRow>> GetProductSalesReportAsync(DateTime startDate, DateTime endDate)
    {
        int startKey = startDate.Year * 10000 + startDate.Month * 100 + startDate.Day;
        int endKey = endDate.Year * 10000 + endDate.Month * 100 + endDate.Day;

        const string sql = @"
            SELECT 
                f.ProductId,
                p.Name as ProductName,
                SUM(f.Quantity) as TotalQuantitySold,
                COUNT(DISTINCT f.OrderId) as TotalOrdersCount,
                SUM(f.TotalPrice) as TotalRevenue
            FROM FactOrders f
            JOIN DimProduct p ON f.ProductId = p.ProductId
            WHERE f.DateKey BETWEEN @startKey AND @endKey
            GROUP BY f.ProductId, p.Name
            ORDER BY TotalRevenue DESC";

        using var conn = GetConnection();
        return await conn.QueryAsync<ProductSalesReportRow>(sql, new { startKey, endKey });
    }

    public async Task<CustomerMetricsSummary?> GetCustomerOrdersReportAsync(Guid customerId)
    {
        const string customerSql = @"
            SELECT 
                c.CustomerId,
                (c.FirstName + ' ' + c.LastName) as CustomerName,
                c.Email,
                COUNT(DISTINCT f.OrderId) as TotalOrdersPlaced,
                SUM(f.TotalPrice) as LifetimeSpend,
                MIN(d.Date) as FirstOrderDate,
                MAX(d.Date) as LastOrderDate
            FROM DimCustomer c
            LEFT JOIN FactOrders f ON c.CustomerId = f.CustomerId
            LEFT JOIN DimDate d ON f.DateKey = d.DateKey
            WHERE c.CustomerId = @customerId
            GROUP BY c.CustomerId, c.FirstName, c.LastName, c.Email";

        const string ordersSql = @"
            SELECT 
                f.OrderId,
                MAX(d.Date) as OrderDate,
                SUM(f.Quantity) as TotalItems,
                MAX(f.OrderTotalAmount) as TotalAmount
            FROM FactOrders f
            JOIN DimDate d ON f.DateKey = d.DateKey
            WHERE f.CustomerId = @customerId
            GROUP BY f.OrderId
            ORDER BY OrderDate DESC";

        using var conn = GetConnection();
        var summary = await conn.QuerySingleOrDefaultAsync<CustomerMetricsSummary>(customerSql, new { customerId });

        if (summary == null) return null;

        summary.Orders = await conn.QueryAsync<CustomerOrdersReportRow>(ordersSql, new { customerId });
        return summary;
    }
}