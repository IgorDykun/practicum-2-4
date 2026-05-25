using FastEndpoints;
using Mediator;
using Nimble.Modulith.Customers.Contracts;
using Nimble.Modulith.Customers.UseCases.Orders.Commands;
using Nimble.Modulith.Customers.Domain.Interfaces;
using Nimble.Modulith.Customers.Domain.OrderAggregate;
using Nimble.Modulith.Email.Contracts;

namespace Nimble.Modulith.Customers.Endpoints.Orders;
public record OrderApiResponse(Guid Id, DateTime OrderDate, decimal TotalAmount);

public class Confirm(IMediator mediator, IRepository<Nimble.Modulith.Customers.Domain.OrderAggregate.Order> repository) : EndpointWithoutRequest<OrderApiResponse>
{
    public override void Configure()
    {
        Post("/orders/{id}/confirm");
        AllowAnonymous();
        Tags("orders");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var orderId = Route<Guid>("id"); 

        var order = await repository.GetByIdAsync(orderId, ct);
        if (order is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var result = await mediator.Send(new ConfirmOrderCommand(orderId), ct);

        if (!result.IsSuccess)
        {
            await Send.ErrorsAsync(statusCode: 400, cancellation: ct);
            return;
        }

        string customerEmail = "customer@test.com";

        var orderCreatedEvent = new OrderCreatedEvent(
            order.Id, order.CustomerId, customerEmail, order.OrderDate, order.TotalAmount
        );
        await mediator.Publish(orderCreatedEvent, ct);

        var emailSubject = order.TotalAmount > 500
            ? $"[VIP] Order Confirmation - {order.Id.ToString()[..8].ToUpper()}"
            : $"Order Confirmation - {order.Id.ToString()[..8].ToUpper()}";

        var emailBody = $"Your order has been confirmed! Total amount: {order.TotalAmount:C}";

        await mediator.Send(new SendEmailCommand(customerEmail, emailSubject, emailBody), ct);

        Response = new OrderApiResponse(order.Id, order.OrderDate, order.TotalAmount);
        await Send.OkAsync(Response, ct);
    }
}