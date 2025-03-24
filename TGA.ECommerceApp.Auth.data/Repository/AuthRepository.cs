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

    public RefreshTokens AddRefreshTokens(RefreshTokens user)
    {
        authDbContext.RefreshToken.Add(user);
        authDbContext.SaveChanges();
        return user;
    }

    public bool DeleteRefreshTokens(string userId)
    {
        var refreshTokens = authDbContext.RefreshToken.Where(rt => rt.UserId == userId).ToList();
        if (refreshTokens.Any())
        {
            authDbContext.RefreshToken.RemoveRange(refreshTokens);
            authDbContext.SaveChanges();
        }
        return true;
    }


    public RefreshTokens GetRefreshTokens(string refreshToken)
    {
        return authDbContext.RefreshToken.FirstOrDefault(rt => rt.Token == refreshToken);
    }

    public async Task<ApplicationUser> GetUserById(string userId)
    {
        return await authDbContext.ApplicationUsers.FirstAsync(u => u.Id == userId);
    }

    public async Task<ApplicationUser> GetUserIdentityByEmail(string email)
    {
        return await authDbContext.ApplicationUsers.FirstAsync(u => u.Email == email);
    }

    public async Task<ApplicationUser> GetUserIdentityByUserName(string userName)
    {
        return await authDbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName.ToLower() == userName.ToLower());
    }

    public bool IsValidUserAsync(ApplicationUser users)
    {
        var u = authDbContext.ApplicationUsers.FirstOrDefault(o => o.Email == users.Email);

        if (u != null)
            return true;
        else
            return false;
    }

    public bool UpdateUserRefreshTokens(RefreshTokens updatedToken)
    {
        var existingToken = authDbContext.RefreshToken.Find(updatedToken.Id);
        if (existingToken == null)
        {
            return false; // Record not found
        }

        // Update properties
        existingToken.Token = updatedToken.Token;
        existingToken.JwtId = updatedToken.JwtId;
        existingToken.IsRevoked = updatedToken.IsRevoked;
        existingToken.IsUsed = updatedToken.IsUsed;

        // Save changes
        authDbContext.SaveChanges();
        return true; // Record updated successfully
    }
}