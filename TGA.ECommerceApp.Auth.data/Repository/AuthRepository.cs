using Microsoft.EntityFrameworkCore;
using TGA.ECommerceApp.Auth.Domain.Interfaces;
using TGA.ECommerceApp.Auth.Domain.Models;
using TGA.ECommerceApp.Product.Data.Context;

namespace TGA.ECommerceApp.Auth.Data.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AuthDbContext authDbContext;

        public AuthRepository(AuthDbContext productDbContext)
        {
            this.authDbContext = productDbContext;
        }

        public async Task<ApplicationUser> GetUserIdentityByEmail(string email)
        {
            return await authDbContext.applicationUsers.FirstAsync(u => u.Email == email);
        }

        public async Task<ApplicationUser> GetUserIdentityByUserName(string userName)
        {
            return await authDbContext.applicationUsers.FirstOrDefaultAsync(u => u.UserName.ToLower() == userName.ToLower());
        }
    }
}