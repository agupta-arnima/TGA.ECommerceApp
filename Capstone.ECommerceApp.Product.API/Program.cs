using AutoMapper;
using Azure;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Security.KeyVault.Secrets;
using Capstone.ECommerceApp.Product.Application;
using Capstone.ECommerceApp.Product.Application.Interfaces;
using Capstone.ECommerceApp.Product.Application.Services;
using Capstone.ECommerceApp.Product.Data.Context;
using Capstone.ECommerceApp.Product.Data.Repository;
using Capstone.ECommerceApp.Product.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

var productDbConnectionStr = builder.Configuration.GetConnectionString("ProductDbConnection");
builder.Services.AddDbContextPool<ProductDbContext>(options =>
{
    options.UseMySql(productDbConnectionStr, ServerVersion.AutoDetect(productDbConnectionStr));
});


// Add Azure Key Vault to configuration
builder.Host.ConfigureAppConfiguration((context, config) =>
{
config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

var builtConfig = config.Build(); // Build to access existing config values
var keyVaultUri = builtConfig["AzureConfiguration:AzureKeyVault:VaultUri"];

    if (!string.IsNullOrEmpty(keyVaultUri))
    {
        try
        {
            var secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());
            config.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Failed to load Azure Key Vault. Using local settings. Error: {ex.Message}");
        }
    }
});


//automapper
IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Add services to the container.
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddApplicationDI();
builder.Services.AddControllers();

// Add CORS services
var allowedOrigins = builder.Configuration["CORS_ALLOWED_ORIGINS"]?.Split(';') ?? new string[] { };
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrderApi",
    builder => builder.WithOrigins(allowedOrigins)
    .AllowAnyHeader()
    .AllowAnyMethod());
});


//AI-Search
var apiKey = builder.Configuration["sackumar6-ai-search-apikey"];
var serviceName = builder.Configuration["sackumar6-ai-search-service"];
var indexName = builder.Configuration["sackumar6-ai-search-index"];

builder.Services.AddSingleton(serviceProvider =>
{
    Uri endpoint = new Uri($"https://{serviceName}.search.windows.net/");
    AzureKeyCredential credential = new AzureKeyCredential(apiKey);

    return new SearchClient(endpoint, indexName, credential);
});

builder.Services.AddSingleton(serviceProvider =>
{
    Uri endpoint = new Uri($"https://{serviceName}.search.windows.net/");
    AzureKeyCredential credential = new AzureKeyCredential(apiKey);

    return new SearchIndexClient(endpoint, credential);
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["ApiSettings:JwtOptions:Issuer"],
                ValidAudience = builder.Configuration["ApiSettings:JwtOptions:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["ApiSettings:JwtOptions:Secret"]))
            };
        });


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Inventory API", Version = "v1" });
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                       Type = ReferenceType.SecurityScheme,
                       Id = "Bearer"
                    }
                },
                new string[] {}
            }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API v1");
    });
}

app.UseHttpsRedirection();


app.UseCors("AllowOrderApi"); // Use the CORS policy


app.UseAuthorization();

app.UseAuthorization();

app.MapControllers();

ApplyMigration();

app.Run();

void ApplyMigration()
{
    using var scope = app.Services.CreateScope();
    var _db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    if (_db.Database.GetPendingMigrations().Any())
        _db.Database.Migrate();
}