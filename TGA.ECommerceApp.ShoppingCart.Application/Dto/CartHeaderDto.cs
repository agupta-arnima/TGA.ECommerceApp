using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TGA.ECommerceApp.ShoppingCart.Application.Dto;

public class CartHeaderDto
{
    public string? UserId { get; set; }
    public string? CouponCode { get; set; }
    public string? Name { get; set; }        
    public string? Phone { get; set; }
    public string? Email { get; set; }

    //Below 2 properties are calculated properties based on CartDetails, Product.Price and Count, Coupon.DiscountAmount
    public double CartTotal { get; set; }
    public double Discount { get; set; }
}
