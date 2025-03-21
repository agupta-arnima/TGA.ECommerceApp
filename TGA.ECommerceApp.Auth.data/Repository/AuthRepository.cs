using Microsoft.EntityFrameworkCore;
using TGA.ECommerceApp.Auth.Domain.Interfaces;
using TGA.ECommerceApp.Auth.Domain.Models;
using TGA.ECommerceApp.Product.Data.Context;

namespace TGA.ECommerceApp.Auth.Data.Repository;

public class AuthRepository : IAuthRepository
{
    private readonly AuthDbContext authDbContext;

    public AuthRepository(AuthDbContext productDbContext)
    {
        this.authDbContext = productDbContext;
    }

    public UserRefreshTokens AddUserRefreshTokens(UserRefreshTokens user)
    {
        authDbContext.userRefreshToken.Add(user);
        authDbContext.SaveChanges();
        return user;
    }

    public void DeleteUserRefreshTokens(string username, string refreshToken)
    {
        var item = authDbContext.userRefreshToken.FirstOrDefault(x => x.UserName == username && x.RefreshToken == refreshToken);
        if (item != null)
        {
            authDbContext.userRefreshToken.Remove(item);
        }
    }

    public UserRefreshTokens GetSavedRefreshTokens(string username, string refreshtoken)
    {

        return authDbContext.userRefreshToken.FirstOrDefault(x => x.UserName == username && x.RefreshToken == refreshtoken && x.IsActive);
    }

    public async Task<ApplicationUser> GetUserIdentityByEmail(string email)
    {
        return await authDbContext.applicationUsers.FirstAsync(u => u.Email == email);
    }

    public async Task<ApplicationUser> GetUserIdentityByUserName(string userName)
    {
        return await authDbContext.applicationUsers.FirstOrDefaultAsync(u => u.UserName.ToLower() == userName.ToLower());
    }

    public bool IsValidUserAsync(ApplicationUser users)
    {
        var u = authDbContext.applicationUsers.FirstOrDefault(o => o.Email == users.Email);

        if (u != null)
            return true;
        else
            return false;
    }
}