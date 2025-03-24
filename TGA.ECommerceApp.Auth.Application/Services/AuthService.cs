using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using TGA.ECommerceApp.Auth.Application.Dto;
using TGA.ECommerceApp.Auth.Application.Interfaces;
using TGA.ECommerceApp.Auth.Domain.Interfaces;
using TGA.ECommerceApp.Auth.Domain.Models;

namespace TGA.ECommerceApp.Auth.Application.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(IAuthRepository authRepository,
                       UserManager<ApplicationUser> userManager,
                       RoleManager<IdentityRole> roleManager,
                       IJwtTokenGenerator jwtTokenGenerator,
                       IOptions<JwtOptions> jwtOptions)
    {
        _authRepository = authRepository;
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<bool> AssignRole(string email, string roleName)
    {
        var user = await _authRepository.GetUserIdentityByEmail(email);
        if (user == null) return false;

        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            await _roleManager.CreateAsync(new IdentityRole(roleName));
        }

        await _userManager.AddToRoleAsync(user, roleName);
        return true;
    }

    public async Task<TokenRequestDto> GetToken(TokenRequest token)
    {
        var refreshToken = _authRepository.GetRefreshTokens(token.RefreshToken);
        if (refreshToken == null) return null;

        var user = await _authRepository.GetUserById(refreshToken.UserId);
        if (user == null) return null;

        return new TokenRequestDto
        {
            UserName = user.UserName,
            RefreshToken = refreshToken.Token,
            Token = refreshToken.Token,
            IsUsed = refreshToken.IsUsed,
            IsRevoked = refreshToken.IsRevoked,
            JwtId = refreshToken.JwtId
        };
    }

    public async Task<UserDto> GetUser(string userId)
    {
        var user = await _authRepository.GetUserIdentityByUserName(userId);
        return user == null ? null : new UserDto
        {
            ID = user.Id,
            Name = user.Name,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };
    }

    public async Task<LoginResponseDto> Login(LoginRequestDto userDTO)
    {
        var user = await _authRepository.GetUserIdentityByUserName(userDTO.UserName);
        if (user == null || !await _userManager.CheckPasswordAsync(user, userDTO.Password))
        {
            return new LoginResponseDto { User = null, Token = null };
        }

        var roles = await _userManager.GetRolesAsync(user);
        var userDto = new UserDto
        {
            ID = user.Id,
            Name = user.Name,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };

        var (token, jwtId) = await _jwtTokenGenerator.GenerateJwtToken(userDto, roles);
        UpdateRefreshToken(userDto, token, jwtId);

        return new LoginResponseDto { User = userDto, Token = token };
    }

    public bool UpdateRefreshToken(UserDto user, TokenRequest token, string jwtId)
    {
        if (_authRepository.DeleteRefreshTokens(user.ID))
        {
            _authRepository.AddRefreshTokens(new RefreshTokens
            {
                AddedDate = DateTime.UtcNow,
                UserId = user.ID,
                ExpiryDate = DateTime.UtcNow,
                IsUsed = false,
                IsRevoked = false,
                Token = token.RefreshToken,
                JwtId = jwtId
            });
        }
        return true;
    }

    public async Task<UserDto> Register(RegistrationRequestDto registrationRequestDto)
    {
        var user = new ApplicationUser
        {
            Name = registrationRequestDto.Name,
            Email = registrationRequestDto.Email,
            NormalizedEmail = registrationRequestDto.Email.ToUpper(),
            UserName = registrationRequestDto.Email,
            PhoneNumber = registrationRequestDto.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
        if (!result.Succeeded) return null;

        if (!string.IsNullOrEmpty(registrationRequestDto.Role))
        {
            await AssignRole(registrationRequestDto.Email, registrationRequestDto.Role);
        }

        var registeredUser = await _authRepository.GetUserIdentityByEmail(user.Email);
        return new UserDto
        {
            ID = registeredUser.Id,
            Name = registeredUser.Name,
            Email = registeredUser.Email,
            PhoneNumber = registeredUser.PhoneNumber
        };
    }

    public async Task<bool> UpdateUserRefreshTokens(TokenRequestDto updatedToken)
    {
        var existingToken = _authRepository.GetRefreshTokens(updatedToken.RefreshToken);
        if (existingToken == null) return false;

        existingToken.JwtId = updatedToken.JwtId;
        existingToken.Token = updatedToken.Token;
        existingToken.IsRevoked = updatedToken.IsRevoked;
        existingToken.IsUsed = updatedToken.IsUsed;
        existingToken.AddedDate = DateTime.UtcNow;

        _authRepository.UpdateUserRefreshTokens(existingToken);
        return true;
    }

    public TokenRequest GenerateJwtToken(UserDto user)
    {
        var (token, jwtId) = _jwtTokenGenerator.GenerateJwtToken(user, null).Result;
        UpdateRefreshToken(user, token, jwtId);
        return token;
    }
}