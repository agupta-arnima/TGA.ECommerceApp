using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TGA.ECommerceApp.Auth.Application.Dto;
using TGA.ECommerceApp.Auth.Application.Interfaces;
using TGA.ECommerceApp.Auth.Domain.Models;

namespace TGA.ECommerceApp.Auth.Application.Services;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtOptions _jwtOptions;

    public JwtTokenGenerator(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<(TokenRequest, string JwtId)> GenerateJwtToken(UserDto user, IEnumerable<string> roles = null)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);

        var claims = GetClaims(user, roles);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddSeconds(300), // Adjust expiration as needed
            Issuer = _jwtOptions.Issuer, // Ensure issuer is set
            Audience = _jwtOptions.Audience, // Ensure audience is set
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature)
        };

        var token = jwtTokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = jwtTokenHandler.WriteToken(token);

        var refreshToken = GenerateRefreshToken(user.ID, token.Id);

        var tokenRequest = new TokenRequest
        {
            Token = jwtToken,
            RefreshToken = refreshToken.Token
        };

        return (tokenRequest, token.Id);
    }

    private List<Claim> GetClaims(UserDto user, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim("Id", user.ID),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (roles != null && roles.Any())
        {
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        }

        return claims;
    }

    private RefreshTokenDto GenerateRefreshToken(string userId, string jwtId)
    {
        return new RefreshTokenDto
        {
            JwtId = jwtId,
            IsUsed = false,
            IsRevoked = false,
            UserId = userId,
            AddedDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddMonths(6),
            Token = GenerateRandomString(35) + Guid.NewGuid()
        };
    }

    private string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}