using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Nimble.Modulith.Reporting.Services;

namespace Nimble.Modulith.Reporting.Endpoints.Reports;

public class OrdersReportRequest
{
    public DateTime StartDate { get; set; } = new(2025, 1, 1);
    public DateTime EndDate { get; set; } = new(2025, 12, 31);
    public string? Format { get; set; }
}

public class OrdersReport : Endpoint<OrdersReportRequest, object>
{
    private readonly IReportService _reportService;

    public OrdersReport(IReportService reportService) => _reportService = reportService;

    public override void Configure()
    {
        Get("/reports/orders");
        AllowAnonymous();
    }

    public override async Task HandleAsync(OrdersReportRequest req, CancellationToken ct)
    {
        var report = await _reportService.GetOrdersReportAsync(req.StartDate, req.EndDate);

        bool isCsv = "csv".Equals(req.Format, StringComparison.OrdinalIgnoreCase) ||
                     HttpContext.Request.Headers.Accept.ToString().Contains("text/csv");

        if (isCsv)
        {
            var csv = CsvFormatter.ToCsv(report.Rows);

            HttpContext.Response.ContentType = "text/csv";
            await HttpContext.Response.WriteAsync(csv, ct);
        }
        else
        {
            await HttpContext.Response.SendAsync(report, statusCode: 200, cancellation: ct);
        }
    }
}