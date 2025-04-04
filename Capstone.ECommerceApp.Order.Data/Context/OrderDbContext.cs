using Microsoft.EntityFrameworkCore;
using Capstone.ECommerceApp.Order.Domain.Models;

namespace Capstone.ECommerceApp.Order.Data.Context;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    public DbSet<OrderHeader> OrderHeaders { get; set; }
    public DbSet<OrderDetails> OrderDetails { get; set; }
}
