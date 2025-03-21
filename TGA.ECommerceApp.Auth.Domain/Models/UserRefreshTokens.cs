﻿using System.ComponentModel.DataAnnotations;

namespace TGA.ECommerceApp.Auth.Domain.Models;

public class UserRefreshTokens
{
    [Key]
    public int Id { get; set; }

    public required string UserName { get; set; }

    public required string RefreshToken { get; set; }

    public bool IsActive { get; set; } = true;
}
