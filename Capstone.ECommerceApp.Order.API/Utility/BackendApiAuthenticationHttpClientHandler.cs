﻿using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace Capstone.ECommerceApp.Order.API.Utility;

public class BackendApiAuthenticationHttpClientHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BackendApiAuthenticationHttpClientHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        //var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
        //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return await base.SendAsync(request, cancellationToken); //continue the pipeline
    }
}