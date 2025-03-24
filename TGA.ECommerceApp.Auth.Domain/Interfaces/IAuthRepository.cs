using TGA.ECommerceApp.Auth.Domain.Models;
namespace TGA.ECommerceApp.Auth.Domain.Interfaces;

public interface IAuthRepository
{
    Task<ApplicationUser> GetUserIdentityByEmail(string email);
    Task<ApplicationUser> GetUserIdentityByUserName(string userName);
    bool IsValidUserAsync(ApplicationUser users);
    RefreshTokens AddRefreshTokens(RefreshTokens user);
    bool UpdateUserRefreshTokens(RefreshTokens user);
    RefreshTokens GetRefreshTokens(string userId);
    bool DeleteRefreshTokens(string userId);
    Task<ApplicationUser> GetUserById(string userId);
}
