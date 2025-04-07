using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.Metrics;
using System.IdentityModel.Tokens.Jwt;
using TGA.ECommerceApp.Auth.Application.Dto;
using TGA.ECommerceApp.Auth.Application.Interfaces;
using TGA.ECommerceApp.Auth.Domain.Events;
using TGA.ECommerceApp.Domain.Core.Bus;

namespace TGA.ECommerceApp.Auth.API.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthAPIController : ControllerBase
{
    private readonly IAuthService authService;
    protected ResponseDto response;
    private readonly IConfiguration configuration;
    private readonly TokenValidationParameters tokenValidationParams;
    private readonly IEventBus messageBus;

    private readonly Counter<int> registrationCounter; // Counter for token generation
    public AuthAPIController(IAuthService authService,
        IConfiguration configuration,
        TokenValidationParameters tokenValidationParams,
        IEventBus messageBus,
        Meter registrationMeterCounter)
    {
        this.authService = authService;
        this.configuration = configuration;
        this.response = new();
        this.tokenValidationParams = tokenValidationParams;
        this.messageBus = messageBus;
        registrationCounter = registrationMeterCounter.CreateCounter<int>("registrations.count",
            description: "Counts the number of registrations");
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationRequestDto userDTO)
    {
        var result = await authService.Register(userDTO);
        if (result != null && result.IsSuccess)
        {
            //
            await messageBus.PublishMessageAsync(new UserRegistrationEvent(userDTO.Email), 
                configuration.GetValue<string>("ApiSettings:RabbitMQ:TopicAndQueueNames:UserRegistrationQueue"));
            registrationCounter.Add(1); // Increment the counter
            return Ok(result);
        }
        else
        {
            response.Message = "Registration Failed.";
            response.Errors = result.Errors ?? new List<string>();
            response.IsSuccess = false;
            return BadRequest(response);
        };
    }

    [HttpPost("login")]
    //[RequireCertificate]
    public async Task<IActionResult> Login(LoginRequestDto userDTO)
    {
        var result = await authService.Login(userDTO);
        if (result == null || result.Token == null)
        {
            response.IsSuccess = false;
            response.Message = "Login Failed";
            return BadRequest(response);
        }
        else
        {

        }
        response.IsSuccess = true;
        response.Message = "Login Successful";
        response.Result = result;
        return Ok(response);
    }

    [HttpPost]
    [Route("RefreshToken")]
    //[RequireCertificate]
    public async Task<AuthResult> RefreshToken([FromBody] TokenRequest tokenRequest)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();

        // Validation 1 - Validation JWT token format
        var tokenInVerification = jwtTokenHandler.ValidateToken(tokenRequest.Token, tokenValidationParams, out var validatedToken);

        // Validation 2 - Validate encryption alg
        if (validatedToken is JwtSecurityToken jwtSecurityToken)
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
            return new AuthResult()
            {
                Success = false,
                Errors = new List<string>() {
                        "Token has not yet expired"
                    }
            };
        }

        // Validation 4 - validate existence of the token
        var storedToken = await authService.GetToken(tokenRequest);
        if (storedToken == null)
        {
            return new AuthResult()
            {
                Success = false,
                Errors = new List<string>() {
                        "Token does not exist"
                    }
            };
        }

        // Validation 5 - validate if used
        if (storedToken.IsUsed)
        {
            return new AuthResult()
            {
                Success = false,
                Errors = new List<string>() {
                        "Token has been used"
                    }
            };
        }

        // Validation 6 - validate if revoked
        if (storedToken.IsRevoked)
        {
            return new AuthResult()
            {
                Success = false,
                Errors = new List<string>() {
                        "Token has been revoked"
                    }
            };
        }

        //Validation 7 - validate the id
        var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

        if (storedToken.JwtId != jti)
        {
            return new AuthResult()
            {
                Success = false,
                Errors = new List<string>() {
                        "Token doesn't match"
                    }
            };
        }

        // update current token 
        storedToken.IsUsed = true;
        await authService.UpdateUserRefreshTokens(storedToken);

        // Generate a new token
        var dbUser = await authService.GetUser(storedToken.UserName);
        var token = authService.GenerateJwtToken(dbUser);

        return new AuthResult()
        {
            Success = true,
            Token = token.Token,
            RefreshToken = token.RefreshToken,
        };
    }

    private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
    {
        var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp).ToUniversalTime();

        return dateTimeVal;
    }
}