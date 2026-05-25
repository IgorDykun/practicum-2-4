using Ardalis.Specification;
using FastEndpoints;

namespace Nimble.Modulith.Customers.Domain.OrderAggregate.Specifications;

public class OrdersByCustomerSpec : Specification<Order>
{
    public OrdersByCustomerSpec(Guid customerId)
    {
        Query.Where(o => o.CustomerId == customerId)
             .Include(o => o.OrderItems); 
    }
}