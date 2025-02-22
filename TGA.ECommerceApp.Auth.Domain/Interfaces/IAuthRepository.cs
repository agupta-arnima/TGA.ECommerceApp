using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGA.ECommerceApp.Auth.Domain.Models;

namespace TGA.ECommerceApp.Auth.Domain.Interfaces
{
    public interface IAuthRepository
    {
        Task<ApplicationUser> GetUserIdentityByEmail(string email);
        Task<ApplicationUser> GetUserIdentityByUserName(string userName);
    }
}
