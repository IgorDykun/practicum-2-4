using Ardalis.Result;
using Mediator;
namespace Nimble.Modulith.Customers.UseCases.Orders.Commands;
public record ConfirmOrderCommand(Guid OrderId) : ICommand<Result<OrderDto>>;