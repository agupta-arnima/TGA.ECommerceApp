namespace Capstone.ECommerceApp.Auth.Application.Dto;

public class LoginResponseDto
{
    public UserDto User { get; set; }
    public TokenRequest Token { get; set; }
}
