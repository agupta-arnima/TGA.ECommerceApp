using Microsoft.EntityFrameworkCore;
using TGA.ECommerceApp.Order.Domain.Models;

namespace TGA.ECommerceApp.Order.Data.Context
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
        }

        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
    }
}
