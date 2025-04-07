using TGA.ECommerceApp.Auth.Application.Dto;

namespace TGA.ECommerceApp.Auth.Application.Interfaces;

public interface IAuthService
{
    Task<ResponseDto> Register(RegistrationRequestDto userDTO);
    Task<LoginResponseDto> Login(LoginRequestDto userDTO);
    Task<bool> AssignRole(string email, string roleName);
    Task<TokenRequestDto> GetToken(TokenRequest token);
    Task<bool> UpdateUserRefreshTokens(TokenRequestDto updatedToken);
    TokenRequest GenerateJwtToken(UserDto user);
    Task<UserDto> GetUser(string  userId);
    bool UpdateRefreshToken(UserDto user, TokenRequest token, string jwtId);
}
