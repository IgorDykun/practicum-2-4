using Ardalis.Result;
using Mediator;

namespace Nimble.Modulith.Customers.UseCases.Orders.Commands;

public record CreateOrderCommand(Guid CustomerId, List<CreateOrderItemDto> Items) : ICommand<Result<OrderDto>>;
public record CreateOrderItemDto(int ProductId, int Quantity);