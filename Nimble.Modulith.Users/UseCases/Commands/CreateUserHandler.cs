using Ardalis.Result;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Nimble.Modulith.Users.Contracts;

namespace Nimble.Modulith.Users.UseCases.Commands;

public class CreateUserHandler(UserManager<IdentityUser> userManager) : ICommandHandler<CreateUserCommand, Result<UserDto>>
{
    public async ValueTask<Result<UserDto>> Handle(CreateUserCommand command, CancellationToken ct)
    {
        var existingUser = await userManager.FindByEmailAsync(command.Email);
        if (existingUser != null)
        {
            return Result<UserDto>.Conflict($"User with email {command.Email} already exists.");
        }

        var user = new IdentityUser
        {
            UserName = command.Email,
            Email = command.Email,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, command.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<UserDto>.Error(errors);
        }

        await userManager.AddToRoleAsync(user, command.Role);

        return Result<UserDto>.Success(new UserDto(user.Id, user.Email!, command.Role));
    }
}