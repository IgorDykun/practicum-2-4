using Ardalis.Result;
using Mediator;

namespace Nimble.Modulith.Users.Contracts;

public record ResetUserPasswordCommand(string Email, string NewPassword) : ICommand<Result>;