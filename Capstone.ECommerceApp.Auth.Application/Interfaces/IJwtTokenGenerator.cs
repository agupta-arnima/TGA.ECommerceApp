using Capstone.ECommerceApp.Auth.Application.Dto;

namespace Capstone.ECommerceApp.Auth.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        Task<(TokenRequest, string JwtId)> GenerateJwtToken(UserDto user, IEnumerable<string> roles);
    }
}
