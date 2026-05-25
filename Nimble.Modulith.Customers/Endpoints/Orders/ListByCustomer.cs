using FastEndpoints;
using Mediator;
using Nimble.Modulith.Customers.UseCases.Orders.Queries;
using Nimble.Modulith.Customers.UseCases.Orders;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Http;

namespace Nimble.Modulith.Customers.Endpoints.Orders;

public class ListByCustomer : EndpointWithoutRequest<List<OrderDto>>
{
    private readonly IMediator _mediator;

    public ListByCustomer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/customers/{CustomerId}/orders");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var customerId = Route<Guid>("CustomerId");
        var query = new GetOrdersByCustomerQuery(customerId);

        var result = await _mediator.Send(query, ct);

        if (result.IsSuccess)
        {
            HttpContext.Response.StatusCode = 200;
            await HttpContext.Response.WriteAsJsonAsync(result.Value, ct);
        }
        else
        {
            await result.ToMinimalApiResult().ExecuteAsync(HttpContext);
        }
    }
}