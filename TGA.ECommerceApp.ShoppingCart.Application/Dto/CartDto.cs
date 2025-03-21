namespace TGA.ECommerceApp.ShoppingCart.Application.Dto;

public class CartDto
{
    public CartHeaderDto CartHeader { get; set; }
    public IEnumerable<CartDetailsDto>? CartDetails { get; set; }
}
