using TGA.ECommerceApp.Auth.Domain.Models;
namespace TGA.ECommerceApp.Auth.Domain.Interfaces;

public interface IAuthRepository
{
    Task<ApplicationUser> GetUserIdentityByEmail(string email);
    Task<ApplicationUser> GetUserIdentityByUserName(string userName);
    bool IsValidUserAsync(ApplicationUser users);
    UserRefreshTokens AddUserRefreshTokens(UserRefreshTokens user);
    bool UpdateUserRefreshTokens(UserRefreshTokens user);
    UserRefreshTokens GetSavedRefreshTokens(string refreshtoken);
    void DeleteUserRefreshTokens(string username, string refreshToken);
}
