using Ardalis.Result;
using Mediator;
using Nimble.Modulith.Customers.Domain.CustomerAggregate;
using Nimble.Modulith.Customers.Domain.Interfaces;
using Nimble.Modulith.Email.Contracts.Events;
using Nimble.Modulith.Users.Contracts;
using Nimble.Modulith.Email.Contracts;

namespace Nimble.Modulith.Customers.UseCases.Customers.Commands.Create;

public class CreateCustomerHandler(IRepository<Customer> repository, IMediator mediator)
    : ICommandHandler<CreateCustomerCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(CreateCustomerCommand request, CancellationToken ct)
    {
        var customer = new Customer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email
        };

        await repository.AddAsync(customer, ct);

        string temporaryPassword = $"Pass_{Guid.NewGuid().ToString()[..12]}!";

        var createUserCommand = new CreateUserCommand(customer.Email, temporaryPassword, "Customer");
        var userResult = await mediator.Send(createUserCommand, ct);

        if (userResult.IsSuccess)
        {
            var emailSubject = "Welcome to Nimble Modulith!";
            var emailBody = $@"Hello {customer.FirstName} {customer.LastName},

Your customer account has been successfully created.
Here are your temporary login credentials to access the system:
Username: {customer.Email}
Password: {temporaryPassword}

Please change your temporary password immediately after your first login.";

            await mediator.Send(new SendEmailCommand(customer.Email, emailSubject, emailBody), ct);
        }

        var @event = new CustomerCreatedEvent(customer.Id, customer.Email);
        await mediator.Publish(@event, ct);

        return Result.Success(customer.Id);
    }
}