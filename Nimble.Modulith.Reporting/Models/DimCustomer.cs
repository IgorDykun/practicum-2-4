namespace Nimble.Modulith.Reporting.Models;

public class DimCustomer
{
    public Guid CustomerId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
}