using TGA.ECommerceApp.Auth.Application.Dto;

namespace TGA.ECommerceApp.Auth.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        Task<(TokenRequest, string JwtId)> GenerateJwtToken(UserDto user, IEnumerable<string> roles);
    }
}
