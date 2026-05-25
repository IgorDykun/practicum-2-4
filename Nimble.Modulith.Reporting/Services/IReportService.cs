namespace Nimble.Modulith.Reporting.Services;

public interface IReportService
{
    Task<OrdersReportSummary> GetOrdersReportAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<ProductSalesReportRow>> GetProductSalesReportAsync(DateTime startDate, DateTime endDate);
    Task<CustomerMetricsSummary?> GetCustomerOrdersReportAsync(Guid customerId);
}