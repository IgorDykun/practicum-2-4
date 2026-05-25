using Ardalis.Result;
using Mediator;
using Nimble.Modulith.Customers.Domain.Interfaces;
using Nimble.Modulith.Customers.Domain.OrderAggregate;
using Nimble.Modulith.Customers.Domain.OrderAggregate.Specifications;

namespace Nimble.Modulith.Customers.UseCases.Orders.Queries;

public record GetOrdersByCustomerQuery(Guid CustomerId) : IQuery<Result<List<OrderDto>>>;
public class GetOrdersByCustomerHandler(IReadRepository<Order> repository)
    : IQueryHandler<GetOrdersByCustomerQuery, Result<List<OrderDto>>>
{
    public async ValueTask<Result<List<OrderDto>>> Handle(GetOrdersByCustomerQuery request, CancellationToken ct)
    {
        var spec = new OrdersByCustomerSpec(request.CustomerId);

        var orders = await repository.ListAsync(spec, ct);

        var dtos = orders.Select(o => new OrderDto(
            o.Id,
            o.OrderDate,
            o.TotalAmount
        )).ToList();

        return Result.Success(dtos);
    }
}