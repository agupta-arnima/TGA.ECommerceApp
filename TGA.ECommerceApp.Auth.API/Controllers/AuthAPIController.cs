using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public AuthAPIController(IAuthService authService, IConfiguration configuration)
        {
            this.authService = authService;            
            this._configuration = configuration;
            this.response = new();
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
    }
}