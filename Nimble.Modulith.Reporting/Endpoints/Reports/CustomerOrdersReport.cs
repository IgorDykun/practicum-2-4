using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Nimble.Modulith.Reporting.Services;

namespace Nimble.Modulith.Reporting.Endpoints.Reports;

public class CustomerOrdersReportRequest
{
    public Guid CustomerId { get; set; }
    public string? Format { get; set; }
}

public class CustomerOrdersReport : Endpoint<CustomerOrdersReportRequest, object>
{
    private readonly IReportService _reportService;

    public CustomerOrdersReport(IReportService reportService) => _reportService = reportService;

    public override void Configure()
    {
        Get("/reports/customers/{customerId}/orders");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CustomerOrdersReportRequest req, CancellationToken ct)
    {
        var report = await _reportService.GetCustomerOrdersReportAsync(req.CustomerId);
        if (report == null)
        {
            HttpContext.Response.StatusCode = 404;
            return;
        }

        bool isCsv = "csv".Equals(req.Format, StringComparison.OrdinalIgnoreCase) ||
                     HttpContext.Request.Headers.Accept.ToString().Contains("text/csv");

        if (isCsv)
        {
            var csv = CsvFormatter.ToCsv(report.Orders);

            HttpContext.Response.ContentType = "text/csv";
            await HttpContext.Response.WriteAsync(csv, ct);
        }
        else
        {
            await HttpContext.Response.SendAsync(report, statusCode: 200, cancellation: ct);
        }
    }
}