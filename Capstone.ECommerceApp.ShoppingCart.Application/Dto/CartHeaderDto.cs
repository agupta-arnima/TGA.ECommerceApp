using System.Text.Json.Serialization;

namespace Capstone.ECommerceApp.ShoppingCart.Application.Dto;

public class CartHeaderDto
{
    public string? UserId { get; set; }
    public string? CouponCode { get; set; }
    [JsonIgnore]
    public string? Name { get; set; }
    [JsonIgnore]
    public string? Phone { get; set; }
    [JsonIgnore]
    public string? Email { get; set; }

    //Below 2 properties are calculated properties based on CartDetails, Product.Price and Count, Coupon.DiscountAmount
    public double CartTotal { get; set; }
    public double Discount { get; set; }
}
