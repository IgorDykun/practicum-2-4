using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Mediator;
using Nimble.Modulith.Customers;
using Nimble.Modulith.Email;
using Nimble.Modulith.Products;
using Nimble.Modulith.Users;
using Nimble.Modulith.Web;
using Serilog;
using Nimble.Modulith.Reporting;

var logger = Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));
builder.AddServiceDefaults();

builder.Services.AddFastEndpoints(o =>
{
    o.Assemblies = new[]
    {
        typeof(Program).Assembly,                                            
        typeof(Nimble.Modulith.Products.Endpoints.Create).Assembly,         
        typeof(Nimble.Modulith.Customers.Endpoints.Orders.Create).Assembly, 
        typeof(Nimble.Modulith.Users.Endpoints.ForgotPassword).Assembly,
        typeof(Nimble.Modulith.Reporting.Endpoints.Reports.OrdersReport).Assembly
    };
})
.AddAuthenticationJwtBearer(s => s.SigningKey = builder.Configuration["Auth:JwtSecret"])
.AddAuthorization()
.SwaggerDocument();

builder.AddReportingModuleServices();

builder.Services.AddEmailModule(builder.Configuration);
builder.Services.AddMediator(options =>
{
    options.ServiceLifetime = ServiceLifetime.Scoped;
    options.PipelineBehaviors =
    [
        typeof(LoggingBehavior<,>)
    ];
});


builder.AddUsersModuleServices(logger);
builder.AddProductsModuleServices(logger);

var customersLogger = LoggerFactory.Create(logging => logging.AddConsole()).CreateLogger("Customers");
builder.AddCustomersModuleServices(customersLogger);


var app = builder.Build();

app.MapDefaultEndpoints();
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints().UseSwaggerGen(); 


await app.EnsureUsersModuleDatabaseAsync();
await app.EnsureProductsModuleDatabaseAsync();
await app.EnsureCustomersModuleDatabaseAsync();
await app.EnsureReportingModuleDatabaseAsync();

app.Run();