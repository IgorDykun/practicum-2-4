using Mediator;
using Nimble.Modulith.Email.Contracts;
using Nimble.Modulith.Email.Infrastructure;

namespace Nimble.Modulith.Email.Infrastructure;

public class SendEmailHandler(IQueueService queueService) : ICommandHandler<SendEmailCommand>
{
    public async ValueTask<Unit> Handle(SendEmailCommand request, CancellationToken ct)
    {
        var message = new EmailMessage(
            request.To,
            request.Subject,
            request.Body
        );

        await queueService.PushAsync(message, ct);

        return Unit.Value;
    }
}