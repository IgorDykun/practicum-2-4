using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var password = builder.AddParameter("sql-password", "YourStrongPassword123!", secret: true);

var sqlServer = builder.AddSqlServer("sqlserver", password)
   .WithDataVolume()
   ;

var reportingDb = sqlServer.AddDatabase("reportingdb");   

var usersDb = sqlServer.AddDatabase("usersdb");
var productsDb = sqlServer.AddDatabase("productsdb");
var customersDb = sqlServer.AddDatabase("customersdb");

var mailhog = builder.AddContainer("mailhog", "mailhog/mailhog")
    .WithHttpEndpoint(port: 37408, targetPort: 8025, name: "ui") 
    .WithEndpoint(port: 1025, targetPort: 1025, name: "smtp");

var webapi = builder.AddProject<Projects.Nimble_Modulith_Web>("webapi")
    .WithReference(usersDb)
    .WithReference(productsDb)
    .WithReference(customersDb)
    .WithReference(reportingDb)
    .WithEnvironment("EmailSettings__SmtpServer", mailhog.GetEndpoint("smtp").Property(EndpointProperty.Host))
    .WithEnvironment("EmailSettings__SmtpPort", mailhog.GetEndpoint("smtp").Property(EndpointProperty.Port))
    .WaitFor(usersDb)
    .WaitFor(productsDb)
    .WaitFor(customersDb)
    .WaitFor(reportingDb);


builder.Build().Run();