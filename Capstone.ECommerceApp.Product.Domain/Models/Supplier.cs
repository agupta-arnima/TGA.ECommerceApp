namespace Capstone.ECommerceApp.Product.Domain.Models;

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ContactNumber { get; set; }
    public string? Email { get; set; }
    public string Address { get; set; }
    public ICollection<ProductInfo> Products { get; set; }
}
