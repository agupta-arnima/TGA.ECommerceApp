using Capstone.ECommerceApp.Infra.Common;

namespace Capstone.ECommerceApp.Auth.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        Task<(TokenRequest, string JwtId)> GenerateJwtToken(UserDto user, IEnumerable<string> roles);
    }
}
