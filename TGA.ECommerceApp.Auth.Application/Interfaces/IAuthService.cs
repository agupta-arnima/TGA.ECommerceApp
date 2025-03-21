using TGA.ECommerceApp.Auth.Application.Dto;
using TGA.ECommerceApp.Auth.Domain.Models;

namespace TGA.ECommerceApp.Auth.Application.Interfaces;

public interface IAuthService
{
    Task<string> Register(RegistrationRequestDto userDTO);
    Task<LoginResponseDto> Login(LoginRequestDto userDTO);
    Task<bool> AssignRole(string email, string roleName);
    Task<TokenRequestDto> GetToken(TokenRequestDto token);
    Task<bool> UpdateUserRefreshTokens(TokenRequestDto updatedToken);
}
