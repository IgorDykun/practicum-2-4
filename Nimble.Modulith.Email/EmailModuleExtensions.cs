using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nimble.Modulith.Email.Infrastructure;
using Mediator;

namespace Nimble.Modulith.Email;

public static class EmailModuleExtensions
{
    public static IServiceCollection AddEmailModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.AddSingleton<IQueueService, ChannelQueueService>();
        services.AddHostedService<EmailBackgroundWorker>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();

        return services;
    }
}