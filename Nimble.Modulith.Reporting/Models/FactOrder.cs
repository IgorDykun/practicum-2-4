namespace Nimble.Modulith.Reporting.Models;

public class FactOrder
{
    public Guid OrderId { get; set; }
    public Guid OrderItemId { get; set; }

    public int DateKey { get; set; }
    public Guid CustomerId { get; set; }
    public int ProductId { get; set; }

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; } 
    public decimal OrderTotalAmount { get; set; } 


    public DimDate DimDate { get; set; } = null!;
    public DimCustomer DimCustomer { get; set; } = null!;
    public DimProduct DimProduct { get; set; } = null!;
}