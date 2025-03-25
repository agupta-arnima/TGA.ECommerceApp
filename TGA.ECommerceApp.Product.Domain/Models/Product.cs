using System.ComponentModel.DataAnnotations;

namespace TGA.ECommerceApp.Product.Domain.Models
{
    public class ProductInfo
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; }

        [Range(1, 1000)]
        public double Price { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public int Stock { get; set; }
        public string ImageUrl { get; set; }
    }
}