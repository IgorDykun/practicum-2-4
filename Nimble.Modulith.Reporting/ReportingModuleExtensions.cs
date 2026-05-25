using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nimble.Modulith.Reporting.Data;
using Nimble.Modulith.Reporting.Services;

namespace Nimble.Modulith.Reporting;

public static class ReportingModuleExtensions
{
    public static IHostApplicationBuilder AddReportingModuleServices(this IHostApplicationBuilder builder)
    {
        builder.AddSqlServerDbContext<ReportingDbContext>("reportingdb");

        builder.Services.AddScoped<IReportService, ReportService>();

        return builder;
    }

    public static async Task<WebApplication> EnsureReportingModuleDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ReportingDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ReportingDbContext>>();
        var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();

        try
        {
            if (env.IsDevelopment())
            {
                logger.LogInformation("Development mode: Recreating reporting database...");
                await context.Database.EnsureDeletedAsync();
                var created = await context.Database.EnsureCreatedAsync();
                if (created)
                {
                    logger.LogInformation("Reporting database successfully created with 365 seeded dates.");
                }
            }
            else
            {
                await context.Database.EnsureCreatedAsync();
                logger.LogInformation("Reporting database verified.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize reporting database");
            throw;
        }

        return app;
    }
}