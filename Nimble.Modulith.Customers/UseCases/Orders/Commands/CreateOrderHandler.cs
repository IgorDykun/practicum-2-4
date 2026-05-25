using Ardalis.Result;
using Mediator;
using Nimble.Modulith.Customers.Domain.Interfaces;
using Nimble.Modulith.Customers.Domain.OrderAggregate;
using Nimble.Modulith.Products.Contracts;

namespace Nimble.Modulith.Customers.UseCases.Orders.Commands;

public class CreateOrderHandler(IRepository<Order> repository, IMediator mediator) : ICommandHandler<CreateOrderCommand, Result<OrderDto>>
{
    public async ValueTask<Result<OrderDto>> Handle(CreateOrderCommand command, CancellationToken ct)
    {
        var order = new Order
        {
            CustomerId = command.CustomerId, 
            OrderDate = DateTime.UtcNow
        };

        foreach (var item in command.Items)
        {
            var productDetails = await mediator.Send(new GetProductDetailsQuery(item.ProductId), ct);

            order.AddItem(item.ProductId, item.Quantity, productDetails.Price);
        }

        await repository.AddAsync(order, ct);
        await repository.SaveChangesAsync(ct);

        var dto = new OrderDto(order.Id, order.OrderDate, order.TotalAmount);

        return Result<OrderDto>.Success(dto);
    }
}