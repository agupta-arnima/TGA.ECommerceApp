namespace Capstone.ECommerceApp.Infra.Common;

public class LoginResponseDto
{
    public UserDto User { get; set; }
    public TokenRequest Token { get; set; }
}
