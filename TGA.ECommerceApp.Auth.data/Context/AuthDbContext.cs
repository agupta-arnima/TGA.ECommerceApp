using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TGA.ECommerceApp.Auth.Domain.Models;

namespace TGA.ECommerceApp.Product.Data.Context
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<RefreshTokens> RefreshToken { get; set; }
    }
}