using System.ComponentModel.DataAnnotations.Schema;

namespace TGA.ECommerceApp.ShoppingCart.Domain.Models
{
    public class CartDetails
    {
        public int ProductId { get; set; }
        public int Count { get; set; }
        
        //[NotMapped]
        //public ProductDto Product { get; set; }

        [ForeignKey("CartHeaderId")]
        public virtual CartHeader CartHeader { get; set; }
    }
}
