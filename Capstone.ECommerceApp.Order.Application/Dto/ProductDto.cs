namespace Capstone.ECommerceApp.Order.Application.Dto;

public class ProductDto
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public string Description { get; set; }
    public string CategoryName { get; set; }
    public string ImageUrl { get; set; }
    public int SupplierId { get; set; }
    public int CategotyId { get; set; }
    public int Stock { get; set; }
}
