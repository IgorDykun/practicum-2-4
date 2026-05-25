using Ardalis.Result;
using Mediator;

namespace Nimble.Modulith.Customers.UseCases.Customers.Commands.Create;

public record CreateCustomerCommand(string FirstName, string LastName, string Email)
    : ICommand<Result<Guid>>;
