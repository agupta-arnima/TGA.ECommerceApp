using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
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
            return productDbContext.Products.ToList();
        }

        public ProductInfo GetProductById(int id)
        {
            return productDbContext.Products.FirstOrDefault(c => c.ProductId == id);
        }
    }
}