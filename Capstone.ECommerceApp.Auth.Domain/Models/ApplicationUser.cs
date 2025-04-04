using Microsoft.AspNetCore.Identity;

namespace Capstone.ECommerceApp.Auth.Domain.Models;

public class ApplicationUser : IdentityUser
{
    //can add more property related to User Identity
    public required string Name { get; set; }
}
