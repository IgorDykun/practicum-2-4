using FastEndpoints;
using Mediator;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Http;
using Nimble.Modulith.Customers.UseCases.Customers.Commands.Create;

namespace Nimble.Modulith.Customers.Endpoints.Customers;

public class Create : Endpoint<CreateCustomerCommand, Guid>
{
    private readonly IMediator _mediator;

    public Create(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/customers");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateCustomerCommand req, CancellationToken ct)
    {
        var result = await _mediator.Send(req, ct);

        if (result.IsSuccess)
        {
            HttpContext.Response.StatusCode = 201;
            await HttpContext.Response.WriteAsJsonAsync(result.Value, ct);
        }
        else
        {
            await result.ToMinimalApiResult().ExecuteAsync(HttpContext);
        }
    }
}