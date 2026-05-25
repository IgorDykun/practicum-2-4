using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nimble.Modulith.Customers.UseCases.Orders;

public record OrderDto(Guid Id, DateTime OrderDate, decimal TotalAmount);
