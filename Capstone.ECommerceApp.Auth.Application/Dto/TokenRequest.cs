﻿namespace Capstone.ECommerceApp.Auth.Application.Dto;

public class TokenRequest
{
    public required string Token { get; set; }

    public required string RefreshToken { get; set; }
}
