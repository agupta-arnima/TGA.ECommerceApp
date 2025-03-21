using TGA.ECommerceApp.Auth.Domain.Models;
namespace TGA.ECommerceApp.Auth.Domain.Interfaces;

public interface IAuthRepository
{
    Task<ApplicationUser> GetUserIdentityByEmail(string email);
    Task<ApplicationUser> GetUserIdentityByUserName(string userName);
    bool IsValidUserAsync(ApplicationUser users);
    UserRefreshTokens AddUserRefreshTokens(UserRefreshTokens user);
    UserRefreshTokens GetSavedRefreshTokens(string username, string refreshtoken);
    void DeleteUserRefreshTokens(string username, string refreshToken);
}
