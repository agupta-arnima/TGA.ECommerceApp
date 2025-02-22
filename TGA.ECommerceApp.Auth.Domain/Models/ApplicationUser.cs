using Microsoft.AspNetCore.Identity;

namespace TGA.ECommerceApp.Auth.Domain.Models
{
    public class ApplicationUser : IdentityUser
    {
        //can add more property related to User Identity
        public string Name { get; set; }
    }
}
