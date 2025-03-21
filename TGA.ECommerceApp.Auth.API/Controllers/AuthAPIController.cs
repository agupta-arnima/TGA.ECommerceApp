using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using TGA.ECommerceApp.Auth.Application.Dto;
using TGA.ECommerceApp.Auth.Application.Interfaces;

namespace TGA.ECommerceApp.Auth.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService authService;
        protected ResponseDto response;
        private readonly IConfiguration _configuration;
        private readonly TokenValidationParameters _tokenValidationParams;
        public AuthAPIController(IAuthService authService,
            IConfiguration configuration,
            TokenValidationParameters tokenValidationParams)
        {
            this.authService = authService;
            this._configuration = configuration;
            this.response = new();
            _tokenValidationParams = tokenValidationParams;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto userDTO)
        {
            var result = await authService.Register(userDTO);
            if (!string.IsNullOrEmpty(result))
            {
                response.IsSuccess = false;
                response.Message = result;
                return BadRequest(response);
            }
            response.IsSuccess = true;
            response.Message = "User created successfully!";
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto userDTO)
        {
            var result = await authService.Login(userDTO);
            if (result == null || string.IsNullOrEmpty(result.Token))
            {
                response.IsSuccess = false;
                response.Message = "Login Failed";
                return BadRequest(response);
            }
            response.IsSuccess = true;
            response.Message = "Login Successful";
            response.Result = result;
            return Ok(response);
        }

        [HttpPost("assignrole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto registrationRequestDto)
        {
            var result = await authService.AssignRole(registrationRequestDto.Email, registrationRequestDto.Role.ToUpper());
            if (!result)
            {
                response.IsSuccess = false;
                response.Message = "Error encountered!";
                return BadRequest(response);
            }
            response.IsSuccess = true;
            response.Message = "Role assigned successfully!";
            return Ok(response);
        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            // Validation 1 - Validation JWT token format
            var tokenInVerification = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParams, out var validatedToken);

            // Validation 2 - Validate encryption alg
            if(validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                if (result == false)
                {
                    return null;
                }
            }

            // Validation 3 - validate expiry date
            var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);

            if (expiryDate > DateTime.UtcNow)
            {
                return BadRequest(new ResponseDto
                {
                    IsSuccess = false,
                    Errors = new List<string>() {
                            "Token has not yet expired"
                        }
                });
            }

            // Validation 4 - validate existence of the token
            var storedToken = await authService.GetToken(tokenRequest);
            if (storedToken == null)
            {
                return BadRequest(new ResponseDto
                {
                    IsSuccess = false,
                    Errors = new List<string>() {
                             "Token does not exist"
                        }
                });
            }

            // Validation 5 - validate if used
            if (storedToken.IsUsed)
            {
                return BadRequest(new ResponseDto
                {
                    IsSuccess = false,
                    Errors = new List<string>() {
                            "Token has been used"
                        }
                });
            }

            // Validation 6 - validate if revoked
            if (storedToken.IsRevoked)
            {
                return BadRequest(new ResponseDto
                {
                    IsSuccess = false,
                    Errors = new List<string>() {
                            "Token has been revoked"
                        }
                });
            }

            //Validation 7 - validate the id
            var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            if (storedToken.JwtId != jti)
            {
                return BadRequest(new ResponseDto
                {
                    IsSuccess = false,
                    Errors = new List<string>() {
                             "Token doesn't match"
                        }
                });
            }

            // update current token 
            storedToken.IsUsed = true;
            await authService.UpdateUserRefreshTokens(storedToken);

            //// Generate a new token
            //var dbUser = await userManager.FindByIdAsync(storedToken.UserId);
            //return await GenerateJwtToken(dbUser);
            return Ok(response);
        }

        private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp).ToUniversalTime();

            return dateTimeVal;
        }
    }
}