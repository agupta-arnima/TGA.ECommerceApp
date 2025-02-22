using Microsoft.EntityFrameworkCore;
using TGA.ECommerceApp.Auth.Domain.Models;

namespace TGA.ECommerceApp.Product.Data.Context
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        public DbSet<ApplicationUser> applicationUsers { get; set; }
    }
}