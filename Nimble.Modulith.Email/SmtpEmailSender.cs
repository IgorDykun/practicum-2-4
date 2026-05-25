using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Nimble.Modulith.Email.Data;

namespace Nimble.Modulith.Email;

public class SmtpEmailSender : IEmailSender
{
    private readonly ILogger<SmtpEmailSender> _logger;
    private readonly EmailSettings _settings;

    public SmtpEmailSender(
        ILogger<SmtpEmailSender> logger,
        IOptions<EmailSettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;
    }

    public async Task SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        var auditLog = new EmailAuditLog
        {
            To = message.To,
            Subject = message.Subject
        };

        try
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(MailboxAddress.Parse(message.From ?? _settings.DefaultFromAddress));
            mimeMessage.To.Add(MailboxAddress.Parse(message.To));
            mimeMessage.Subject = message.Subject;
            mimeMessage.Body = new TextPart("plain") { Text = message.Body };

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, _settings.EnableSsl, cancellationToken);

            if (!string.IsNullOrEmpty(_settings.Username))
            {
                await client.AuthenticateAsync(_settings.Username, _settings.Password, cancellationToken);
            }

            await client.SendAsync(mimeMessage, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            auditLog.IsSuccess = true;
            _logger.LogInformation("Email sent successfully to {To}", message.To);
        }
        catch (Exception ex)
        {
            auditLog.IsSuccess = false;
            _logger.LogError(ex, "Failed to send email to {To}", message.To);
            throw;
        }
        finally
        {
            _logger.LogWarning("AUDIT LOG SAVED: [To: {To}, Subject: {Subject}, Success: {Success}, Time: {Time}]",
                auditLog.To, auditLog.Subject, auditLog.IsSuccess, auditLog.SentAt);
        }
    }
}