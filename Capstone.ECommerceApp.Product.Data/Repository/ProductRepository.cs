using Microsoft.EntityFrameworkCore;
using Capstone.ECommerceApp.Product.Data.Context;
using Capstone.ECommerceApp.Product.Domain.Interfaces;
using Capstone.ECommerceApp.Product.Domain.Models;

namespace Capstone.ECommerceApp.Product.Data.Repository
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
                .Include(p => p.Category)
                .ToList();
        }

        public ProductInfo GetProductById(int id)
        {
            return productDbContext.Products
                .Include(p => p.Category)
                .FirstOrDefault(c => c.ProductId == id);
        }

        public bool DeleteProduct(int id)
        {
            productDbContext.Products.Remove(GetProductById(id));
            productDbContext.SaveChanges();
            return true;
        }

        public Category GetCategoryById(int id)
        {
            return productDbContext.Categories.FirstOrDefault(p => p.Id == id);
        }

        public Category CreateCategory(Category category)
        {
            productDbContext.Categories.Add(category);
            productDbContext.SaveChanges();
            return category;
        }

        public Category UpdateCategory(Category category)
        {
            throw new NotImplementedException();
        }

        public Category DeleteCategory(int id)
        {
            throw new NotImplementedException();
        }

        public Supplier GetSupplierById(int id)
        {
            return productDbContext.Suppliers.FirstOrDefault(p => p.Id == id);
        }

        public Supplier CreateSupplier(Supplier supplier)
        {
            productDbContext.Suppliers.Add(supplier);
            productDbContext.SaveChanges();
            return supplier;
        }

        public Supplier UpdateSupplier(Supplier supplier)
        {
            throw new NotImplementedException();
        }

        public Supplier DeleteSupplier(int id)
        {
            throw new NotImplementedException();
        }

        public ProductInfo CreateProduct(ProductInfo productInfo)
        {
            var product = productDbContext.Products.Add(productInfo);
            productDbContext.SaveChanges();
            return product.Entity;
        }

        public async Task UpdateProduct(ProductInfo product)
        {
            var existingProduct = await productDbContext.Products.FindAsync(product.ProductId);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.SupplierId = product.SupplierId;
                existingProduct.Stock = product.Stock;
                existingProduct.ImageUrl = product.ImageUrl;

                productDbContext.Products.Update(existingProduct);
                await productDbContext.SaveChangesAsync();
            }
        }
    }
}