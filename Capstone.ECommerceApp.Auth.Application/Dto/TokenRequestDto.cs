namespace Capstone.ECommerceApp.Auth.Application.Dto;

public class TokenRequestDto
{
    public required string UserName { get; set; }
    public required string Token { get; set; }
    public required string RefreshToken { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsRevoked { get; set; } = false;

    public bool IsUsed { get; set; } = false;
    public string JwtId { get; set; }
}
