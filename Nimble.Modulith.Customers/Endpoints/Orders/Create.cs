using FastEndpoints;
using Mediator;
using Nimble.Modulith.Customers.UseCases.Orders.Commands;

namespace Nimble.Modulith.Customers.Endpoints.Orders;

public class CreateOrderRequest
{
    public Guid CustomerId { get; set; } 
    public List<CreateOrderItemRequest> Items { get; set; } = [];
}

public class CreateOrderItemRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class Create(IMediator mediator) : Endpoint<CreateOrderRequest, OrderApiResponse>
{
    public override void Configure()
    {
        Post("/orders");
        AllowAnonymous();
        Tags("orders");
    }

    public override async Task HandleAsync(CreateOrderRequest req, CancellationToken ct)
    {
        var command = new CreateOrderCommand(
            req.CustomerId,
            req.Items.Select(i => new CreateOrderItemDto(i.ProductId, i.Quantity)).ToList()
        );

        var result = await mediator.Send(command, ct);

        if (!result.IsSuccess)
        {
            await Send.ErrorsAsync(statusCode: 400, cancellation: ct);
            return;
        }

        Response = new OrderApiResponse(result.Value.Id, result.Value.OrderDate, result.Value.TotalAmount);
        await Send.OkAsync(Response, ct);
    }
}