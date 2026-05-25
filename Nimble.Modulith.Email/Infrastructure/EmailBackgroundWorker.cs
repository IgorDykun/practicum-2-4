using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Nimble.Modulith.Email.Infrastructure;

public class EmailBackgroundWorker : BackgroundService
{
    private readonly IQueueService _queue;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EmailBackgroundWorker> _logger;

    public EmailBackgroundWorker(
        IQueueService queue,
        IServiceProvider serviceProvider,
        ILogger<EmailBackgroundWorker> logger)
    {
        _queue = queue;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Email Background Worker запущений.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var message = await _queue.PopAsync(stoppingToken);

                _logger.LogInformation("Знайдено лист для {To}, відправляємо...", message.To);

                using var scope = _serviceProvider.CreateScope();
                var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

                await emailSender.SendEmailAsync(message, stoppingToken);
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при обробці листа з черги.");
            }
        }
    }
}