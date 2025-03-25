using Microsoft.EntityFrameworkCore;
using TGA.ECommerceApp.Product.Data.Context;
using TGA.ECommerceApp.Product.Domain.Interfaces;
using TGA.ECommerceApp.Product.Domain.Models;

namespace TGA.ECommerceApp.Product.Data.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext productDbContext;

        public ProductRepository(ProductDbContext productDbContext)
        {
            this.productDbContext = productDbContext;
        }

        public IEnumerable<ProductInfo> GetProducts()
        {
            return productDbContext.Products
                .Include(p=>p.Category)
                .ToList();
        }

        public ProductInfo GetProductById(int id)
        {
            return productDbContext.Products
                .Include(p=>p.Category)
                .FirstOrDefault(c => c.ProductId == id);
        }

        public bool DeleteProduct(int id) { 
            productDbContext.Products.Remove(GetProductById(id));
            productDbContext.SaveChanges();
            return true;
        }
    }
}