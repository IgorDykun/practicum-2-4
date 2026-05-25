using Ardalis.Result;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Nimble.Modulith.Users.Contracts;

namespace Nimble.Modulith.Users.UseCases.Commands;

public class ResetUserPasswordHandler(UserManager<IdentityUser> userManager) : ICommandHandler<ResetUserPasswordCommand, Result>
{
    public async ValueTask<Result> Handle(ResetUserPasswordCommand command, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(command.Email);
        if (user == null)
        {
            return Result.Success();
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, token, command.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Error(errors);
        }

        return Result.Success();
    }
}