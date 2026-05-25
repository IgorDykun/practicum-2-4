using MailKit.Net.Smtp;
using MimeKit;
using Mediator;
using Nimble.Modulith.Email.Contracts.Events;

namespace Nimble.Modulith.Email.Handlers;

public class SendEmailOnCustomerCreatedHandler : INotificationHandler<CustomerCreatedEvent>
{
    public async ValueTask Handle(CustomerCreatedEvent notification, CancellationToken ct)
    {
        Console.WriteLine($"[EMAIL] Спроба відправки на: {notification.Email}");

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Nimble Modulith", "system@nimble.com"));
        message.To.Add(new MailboxAddress("Customer", notification.Email));
        message.Subject = "Welcome to Nimble!";
        message.Body = new TextPart("plain") { Text = "Вітаємо у нашій системі!" };

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync("localhost", 1025, false, ct);
            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);

            Console.WriteLine("[EMAIL] Успішно відправлено в MailHog!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EMAIL] Помилка відправки: {ex.Message}");
        }
    }
}