using System.Threading.Channels;

namespace Nimble.Modulith.Email.Infrastructure;

public interface IQueueService
{
    ValueTask PushAsync(EmailMessage message, CancellationToken ct = default);
    ValueTask<EmailMessage> PopAsync(CancellationToken ct = default);
}

public class ChannelQueueService : IQueueService
{
    private readonly Channel<EmailMessage> _channel = Channel.CreateUnbounded<EmailMessage>();

    public ValueTask PushAsync(EmailMessage message, CancellationToken ct = default)
        => _channel.Writer.WriteAsync(message, ct);

    public ValueTask<EmailMessage> PopAsync(CancellationToken ct = default)
        => _channel.Reader.ReadAsync(ct);
}