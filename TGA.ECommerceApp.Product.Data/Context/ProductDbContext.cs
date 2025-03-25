using Microsoft.EntityFrameworkCore;
using TGA.ECommerceApp.Product.Domain.Models;

namespace TGA.ECommerceApp.Product.Data.Context
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {
        }

        public DbSet<ProductInfo> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductInfo>()
                        .HasOne(p => p.Category)
                        .WithMany(c => c.Products)
                        .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<ProductInfo>()
                        .HasOne(p => p.Supplier)
                        .WithMany(s => s.Products)
                        .HasForeignKey(p => p.SupplierId);

            modelBuilder.Entity<Category>().HasData(
                            new Category { Id = 1, Name = "Fruit", Description = "Fresh fruits" },
                            new Category { Id = 2, Name = "Vegetable", Description = "Fresh vegetables" }
                            );

            modelBuilder.Entity<Supplier>().HasData(
                            new Supplier { Id = 1, Name = "Supplier A", ContactNumber = "1234567890", Email = "supplierA@example.com", Address = "123 Street, City" },
                            new Supplier { Id = 2, Name = "Supplier B", ContactNumber = "0987654321", Email = "supplierB@example.com", Address = "456 Avenue, City" }
                        );


            modelBuilder.Entity<ProductInfo>().HasData(
                        new ProductInfo
                        {
                            ProductId = 1,
                            Name = "Apple",
                            Price = 15,
                            Description = "Quisque vel lacus ac magna, vehicula sagittis ut non lacus.",
                            ImageUrl = "https://placehold.co/603x403",
                            CategoryId = 1,
                            SupplierId = 1,
                            Stock = 100
                        },
                        new ProductInfo
                        {
                            ProductId = 2,
                            Name = "Orange",
                            Price = 13.99,
                            Description = "Quisque vel lacus ac magna, vehicula sagittis ut non lacus.",
                            ImageUrl = "https://placehold.co/602x402",
                            CategoryId = 1,
                            SupplierId = 2,
                            Stock = 150
                        },
                        new ProductInfo
                        {
                            ProductId = 3,
                            Name = "Banana",
                            Price = 10.99,
                            Description = "Quisque vel lacus ac magna, vehicula sagittis ut non lacus.",
                            ImageUrl = "https://placehold.co/601x401",
                            CategoryId = 1,
                            SupplierId = 1,
                            Stock = 200
                        },
                        new ProductInfo
                        {
                            ProductId = 4,
                            Name = "Pineapple",
                            Price = 15,
                            Description = "Quisque vel lacus ac magna, vehicula sagittis ut non lacus.",
                            ImageUrl = "https://placehold.co/600x400",
                            CategoryId = 1,
                            SupplierId = 2,
                            Stock = 50
                        }
                    );
        }
    }
}
