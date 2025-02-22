using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGA.ECommerceApp.Auth.Application.Dto;

namespace TGA.ECommerceApp.Auth.Application.Interfaces
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDto userDTO);
        Task<LoginResponseDto> Login(LoginRequestDto userDTO);
        Task<bool> AssignRole(string email, string roleName);
    }
}
