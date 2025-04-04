namespace Capstone.ECommerceApp.Product.Domain.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ICollection<ProductInfo> Products { get; set; }
}
