using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TGA.ECommerceApp.Auth.Application.Interfaces;
using TGA.ECommerceApp.Auth.Application.Services;
using TGA.ECommerceApp.Auth.Data.Repository;
using TGA.ECommerceApp.Auth.Domain.Interfaces;
using TGA.ECommerceApp.Auth.Domain.Models;
using TGA.ECommerceApp.Product.Data.Context;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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