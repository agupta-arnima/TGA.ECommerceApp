using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authentication.Certificate;
using System.Text;
using Capstone.ECommerceApp.Auth.Application.Interfaces;
using Capstone.ECommerceApp.Auth.Application.Services;
using Capstone.ECommerceApp.Auth.Data.Repository;
using Capstone.ECommerceApp.Auth.Domain.Interfaces;
using Capstone.ECommerceApp.Auth.Domain.Models;
using Capstone.ECommerceApp.Product.Data.Context;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Capstone.ECommerceApp.Infra.Bus;
using Capstone.ECommerceApp.Domain.Core.Bus;

// Variable for Aspire DashBoard
var registrationMeterCounter = new Meter("OTel.Tempest", "1.0.0");
var registrationCounter = registrationMeterCounter.CreateCounter<int>("registrations.count", description: "Counts the number of registrations");
// Custom ActivitySource for the application
var checkoutActivitySource = new ActivitySource("OTel.Example");


var builder = WebApplication.CreateBuilder(args);

//Load the certificate
//var rootCert = new X509Certificate2(@"C:\Users\sackumar6\source\repos\TGA_Training_Code_samples-mTLS\TGA_Training_Code_samples-mTLS\Certs\server.pfx", "1234"); // Adjust path and password
var rootCert = GetCertificate(Environment.GetEnvironmentVariable("CERT_THUMBPRINT"));

builder.WebHost.UseKestrel(options =>
{
    options.ListenAnyIP(7002, listenOptions =>
    {
        listenOptions.UseHttps(httpsOptions =>
        {
            httpsOptions.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
            httpsOptions.CheckCertificateRevocation = false;
            httpsOptions.ServerCertificate = rootCert; // Use the loaded certificate
        });
    });
});

var authDbConnectionStr = builder.Configuration.GetConnectionString("AuthDbConnection");
builder.Services.AddDbContextPool<AuthDbContext>(options =>
{
    options.UseMySql(authDbConnectionStr, ServerVersion.AutoDetect(authDbConnectionStr));
});

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AuthDbContext>().AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

builder.Services.AddSingleton(registrationMeterCounter);

//Rabbit MQ
builder.Services.Configure<RabbitMQSetting>(builder.Configuration.GetSection("ApiSettings:RabbitMQ"));
builder.Services.AddScoped(typeof(IEventBus), typeof(RabbitMQBus));

builder.Services.AddControllers();

//Added mTLS auth configuration
builder.Services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
    .AddCertificate(options =>
    {
        options.AllowedCertificateTypes = CertificateTypes.All;
        options.ChainTrustValidationMode = X509ChainTrustMode.System;
        options.RevocationMode = X509RevocationMode.NoCheck;
        options.Events = new CertificateAuthenticationEvents
        {
            OnCertificateValidated = context =>
            {
                if (context.ClientCertificate != null)
                {
                    context.Success();
                }
                else
                {
                    context.Fail("Invalid certificate");
                }

                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                context.Fail("Invalid certificate");
                return Task.CompletedTask;
            }
        };
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var key = Encoding.ASCII.GetBytes(builder.Configuration["ApiSettings:JwtOptions:Secret"]);

var tokenValidationParams = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = false,
    RequireExpirationTime = false,
    ClockSkew = TimeSpan.Zero
};

builder.Services.AddSingleton(tokenValidationParams);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
    {
        policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireClaim(ClaimTypes.NameIdentifier);
    });
});


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(jwt =>
{
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = tokenValidationParams;
    // We have to hook the OnMessageReceived event in order to
    // allow the JWT authentication handler to read the access
    // token from the query string when a WebSocket or 
    // Server-Sent Events request comes in.
    jwt.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            if (!string.IsNullOrEmpty(accessToken) &&
                (context.HttpContext.WebSockets.IsWebSocketRequest || context.Request.Headers["Accept"] == "text/event-stream"))
            {
                context.Token = context.Request.Query["access_token"];
            }

            // If the request is for our hub...
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                (path.StartsWithSegments("/ordershub")))
            {
                // Read the token out of the query string
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

//Otel config
// Setup logging to be exported via OpenTelemetry
builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeFormattedMessage = true;
    logging.IncludeScopes = true;
});

var otel = builder.Services.AddOpenTelemetry();

// Add Metrics for ASP.NET Core and our custom metrics and export via OTLP
otel.WithMetrics(metrics =>
{
    // Metrics provider from OpenTelemetry
    metrics.AddAspNetCoreInstrumentation();
    //Our custom metrics
    metrics.AddMeter(registrationMeterCounter.Name);
    // Metrics provides by ASP.NET Core in .NET 8
    metrics.AddMeter("Microsoft.AspNetCore.Hosting");
    metrics.AddMeter("Microsoft.AspNetCore.Server.Kestrel");
});

// Add Tracing for ASP.NET Core and our custom ActivitySource and export via OTLP
otel.WithTracing(tracing =>
{
    tracing.AddAspNetCoreInstrumentation();
    tracing.AddHttpClientInstrumentation();
    //tracing.AddGrpcCoreInstrumentation();
    tracing.AddSource(checkoutActivitySource.Name);
});

// Export OpenTelemetry data via OTLP, using env vars for the configuration
var OtlpEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
if (OtlpEndpoint != null)
{
    otel.UseOtlpExporter();
}

//end of otel config

var app = builder.Build();

/*
app.MapGet("/", SimulatedCheckout);
async Task<string> SimulatedCheckout(ILogger<Program> logger)
{
    // Create a new Activity scoped to the method
    using var activity = checkoutActivitySource.StartActivity("CheckoutActivity");

    // Log a message
    logger.LogInformation("Sending checkout");

    // Increment the custom counter
    countCheckouts.Add(1);

    // Add a tag to the Activity
    activity?.SetTag("checkout", "Hello World!");
    activity?.SetTag("userID", "123");
    activity?.SetTag("cartID", "ABC");

    return "Hello World!";
}
*/
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

ApplyMigration();

app.Run();

void ApplyMigration()
{
    using var scope = app.Services.CreateScope();
    var _db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    if (_db.Database.GetPendingMigrations().Any())
        _db.Database.Migrate();
}

static X509Certificate2? GetCertificate(string certThumbprint)
{
    if (string.IsNullOrEmpty(certThumbprint))
    {
        Console.WriteLine("Certificate thumbprint is not set.");
        return null;
    }

    try
    {
        var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
        store.Open(OpenFlags.ReadOnly);
        var certs = store.Certificates.Find(X509FindType.FindByThumbprint, certThumbprint, false);
        store.Close();

        if (certs.Count == 0)
        {
            Console.WriteLine("Certificate not found in the store.");
            return null;
        }

        Console.WriteLine("Certificate found and loaded.");
        return certs[0];
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error loading certificate: {ex.Message}");
        return null;
    }
}