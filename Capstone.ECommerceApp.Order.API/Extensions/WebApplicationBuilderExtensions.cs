using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Capstone.ECommerceApp.Order.API.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddAppAuthentication(this WebApplicationBuilder builder)
    {
        var settingSections = builder.Configuration.GetSection("ApiSettings");

        var secret = settingSections.GetValue<string>("Secret"); //builder.Configuration.GetValue<string>("ApiSettings:Secret")
        var issuer = settingSections.GetValue<string>("Issuer");
        var audience = settingSections.GetValue<string>("Audience");

        var key = Encoding.ASCII.GetBytes(secret);
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(x =>
                {
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                });

        return builder;
    }
}
