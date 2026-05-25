using Ardalis.Result;
using Mediator;
using Nimble.Modulith.Customers.Domain.Interfaces;
using Nimble.Modulith.Customers.Domain.OrderAggregate;

namespace Nimble.Modulith.Customers.UseCases.Orders.Commands;

public class ConfirmOrderHandler(IRepository<Order> repository) : ICommandHandler<ConfirmOrderCommand, Result<OrderDto>>
{
    public async ValueTask<Result<OrderDto>> Handle(ConfirmOrderCommand command, CancellationToken ct)
    {
        var order = await repository.GetByIdAsync(command.OrderId, ct);
        if (order is null) return Result<OrderDto>.NotFound($"Order with ID {command.OrderId} not found");

        await repository.UpdateAsync(order, ct);
        await repository.SaveChangesAsync(ct);

        var dto = new OrderDto(order.Id, order.OrderDate, order.TotalAmount);
        return Result<OrderDto>.Success(dto);
    }
}