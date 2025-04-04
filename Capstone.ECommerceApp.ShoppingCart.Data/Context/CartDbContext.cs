using Capstone.ECommerceApp.ShoppingCart.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Capstone.ECommerceApp.ShoppingCart.Data.Context;

public class CartDbContext : DbContext
{
    public CartDbContext(DbContextOptions<CartDbContext> options) : base(options)
    {
    }

    public DbSet<CartHeader> CartHeaders { get; set; }
    public DbSet<CartDetails> CartDetails { get; set; }
}