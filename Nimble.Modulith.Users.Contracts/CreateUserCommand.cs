using Ardalis.Result;
using Mediator;

namespace Nimble.Modulith.Users.Contracts;

public record CreateUserCommand(string Email, string Password, string Role) : ICommand<Result<UserDto>>;
public record UserDto(string Id, string Email, string Role);