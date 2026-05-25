using FastEndpoints;
using Mediator;
using Nimble.Modulith.Users.Contracts;
using Nimble.Modulith.Email.Contracts;

namespace Nimble.Modulith.Users.Endpoints;

public record ForgotPasswordRequest(string Email);

public class ForgotPassword(IMediator mediator) : Endpoint<ForgotPasswordRequest>
{
    public override void Configure()
    {
        Post("/users/forgot-password");
        AllowAnonymous();
        Tags("users");
    }

    public override async Task HandleAsync(ForgotPasswordRequest req, CancellationToken ct)
    {
        string newPassword = $"Reset_{Guid.NewGuid().ToString()[..12]}!";

        var result = await mediator.Send(new ResetUserPasswordCommand(req.Email, newPassword), ct);

        if (!result.IsSuccess)
        {
            await Send.ErrorsAsync(statusCode: 400, cancellation: ct);
            return;
        }

        var emailSubject = "Password Reset Request";
        var emailBody = $@"Your password has been reset. 
Your new temporary password is: {newPassword}
Please change it as soon as possible.";

        await mediator.Send(new SendEmailCommand(req.Email, emailSubject, emailBody), ct);

        await Send.OkAsync(ct);
    }
}