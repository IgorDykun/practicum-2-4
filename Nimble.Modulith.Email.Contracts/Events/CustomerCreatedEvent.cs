using Mediator;

namespace Nimble.Modulith.Email.Contracts.Events; 

public record CustomerCreatedEvent(Guid CustomerId, string Email) : INotification;