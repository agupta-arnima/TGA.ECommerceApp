using Microsoft.AspNetCore.Identity;
using TGA.ECommerceApp.Auth.Application.Dto;
using TGA.ECommerceApp.Auth.Application.Interfaces;
using TGA.ECommerceApp.Auth.Domain.Interfaces;
using TGA.ECommerceApp.Auth.Domain.Models;

namespace TGA.ECommerceApp.Auth.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository authRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IJwtTokenGenerator jwtTokenGenerator;

        public AuthService(IAuthRepository authRepository, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator)
        {
            this.authRepository = authRepository;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var applicationUser = await authRepository.GetUserIdentityByEmail(email);
            if (applicationUser != null)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
                await userManager.AddToRoleAsync(applicationUser, roleName);
                return true;
            }
            return false;
        }

        public async Task<TokenRequestDto> GetToken(TokenRequestDto token)
        {
            var refreshToken = authRepository.GetSavedRefreshTokens(token.RefreshToken);
            return new TokenRequestDto
            {
                UserName = refreshToken.UserName,
                RefreshToken = refreshToken.RefreshToken,
                Token = refreshToken.Token,
                IsUsed = refreshToken.IsUsed,
                IsRevoked = refreshToken.IsRevoked,
                JwtId = token.JwtId
            };
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto userDTO)
        {
            //var identityUser = await userManager.FindByNameAsync(userDTO.UserName);
            var applicationUser = await authRepository.GetUserIdentityByUserName(userDTO.UserName);

            var isValid = await userManager.CheckPasswordAsync(applicationUser, userDTO.Password);

            if (applicationUser == null || !isValid)
            {
                return new LoginResponseDto
                {
                    User = null,
                    Token = ""
                };
            }
            //If user is found then generate the Jwt token
            var roles = await userManager.GetRolesAsync(applicationUser);

            return new LoginResponseDto
            {
                User = new UserDto
                {
                    ID = applicationUser.Id,
                    Name = applicationUser.Name,
                    Email = applicationUser.Email,
                    PhoneNumber = applicationUser.PhoneNumber
                },
                Token = jwtTokenGenerator.GenerateToken(applicationUser, roles)
            };
        }

        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            ApplicationUser applicationUser = new()
            {
                Name = registrationRequestDto.Name,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                UserName = registrationRequestDto.Email,
                PhoneNumber = registrationRequestDto.PhoneNumber
            };
            try
            {
                var result = await userManager.CreateAsync(applicationUser, registrationRequestDto.Password);
                if (result.Succeeded)
                {
                    var userToReturn = await authRepository.GetUserIdentityByEmail(applicationUser.Email);
                    UserDto userDTO = new()
                    {
                        ID = userToReturn.Id,
                        Name = userToReturn.Name,
                        Email = userToReturn.Email,
                        PhoneNumber = userToReturn.PhoneNumber
                    };
                    return ""; //No error
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<bool> UpdateUserRefreshTokens(TokenRequestDto updatedToken)
        {
            var existingToken = authRepository.GetSavedRefreshTokens(updatedToken.RefreshToken);
            if (existingToken == null)
            {
                return Task.FromResult(false);
            }
            existingToken.UserName = updatedToken.UserName;
            existingToken.JwtId = updatedToken.JwtId;
            existingToken.RefreshToken = updatedToken.RefreshToken;
            existingToken.Token = updatedToken.Token;
            existingToken.IsActive = updatedToken.IsActive;
            existingToken.IsRevoked = updatedToken.IsRevoked;
            existingToken.IsUsed = updatedToken.IsUsed;

            authRepository.UpdateUserRefreshTokens(existingToken);
            return Task.FromResult(true);
        }
    }
}