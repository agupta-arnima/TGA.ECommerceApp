using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capstone.ECommerceApp.ShoppingCart.Domain.Models;

public class CartDetails
{
    [Key]
    public int CartDetailsId { get; set; }
    public int CartHeaderId { get; set; }
    public int ProductId { get; set; }
    public int Count { get; set; }
    
    //[NotMapped]
    //public ProductDto Product { get; set; }

    [ForeignKey("CartHeaderId")]
    public virtual CartHeader CartHeader { get; set; }
}
