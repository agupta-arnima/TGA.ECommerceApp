namespace Capstone.ECommerceApp.Infra.Common;

public class TokenRequest
{
    public required string Token { get; set; }

    public required string RefreshToken { get; set; }
}
