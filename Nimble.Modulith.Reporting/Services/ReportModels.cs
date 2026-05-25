namespace Nimble.Modulith.Reporting.Services;

public class OrdersReportRow
{
    public Guid OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public string CustomerName { get; set; } = null!;
    public int UniqueProductsCount { get; set; }
    public decimal TotalAmount { get; set; }
}

public class OrdersReportSummary
{
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageOrderValue { get; set; }
    public IEnumerable<OrdersReportRow> Rows { get; set; } = Array.Empty<OrdersReportRow>();
}

public class ProductSalesReportRow
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public int TotalQuantitySold { get; set; }
    public int TotalOrdersCount { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class CustomerOrdersReportRow
{
    public Guid OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public int TotalItems { get; set; }
    public decimal TotalAmount { get; set; }
}

public class CustomerMetricsSummary
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int TotalOrdersPlaced { get; set; }
    public decimal LifetimeSpend { get; set; }
    public DateTime? FirstOrderDate { get; set; }
    public DateTime? LastOrderDate { get; set; }
    public IEnumerable<CustomerOrdersReportRow> Orders { get; set; } = Array.Empty<CustomerOrdersReportRow>();
}