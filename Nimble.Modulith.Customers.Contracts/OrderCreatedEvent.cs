using System;
using System.Collections.Generic;
using Mediator;

namespace Nimble.Modulith.Customers.Contracts;

public record OrderCreatedEvent(
    Guid OrderId,
    Guid CustomerId,
    string CustomerEmail,
    DateTime OrderDate,
    decimal TotalAmount
) : INotification;