using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capstone.ECommerceApp.ShoppingCart.Domain.Models;

public class CartHeader
{
    public string? UserId { get; set; }
    public string? CouponCode { get; set; }
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }

    [NotMapped]
    public double CartTotal { get; set; }

    [NotMapped]
    public double Discount { get; set; }
    public IEnumerable<CartDetails> CartDetails { get; set; }
}
