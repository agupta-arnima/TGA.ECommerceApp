namespace TGA.ECommerceApp.Product.Domain.Models;

public class Supplier
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string ContactNumber { get; set; }
    public string? Email { get; set; }
    public required string Address { get; set; }
    public ICollection<ProductInfo> Products { get; set; }
}
