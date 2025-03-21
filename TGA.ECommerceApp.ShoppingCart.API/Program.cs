using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Polly;
using TGA.ECommerceApp.ShoppingCart.API.Extensions;
using TGA.ECommerceApp.ShoppingCart.API.Utility;
using TGA.ECommerceApp.ShoppingCart.Application;
using TGA.ECommerceApp.ShoppingCart.Application.Interfaces;
using TGA.ECommerceApp.ShoppingCart.Application.Services;
using TGA.ECommerceApp.ShoppingCart.Data.Context;
using TGA.ECommerceApp.ShoppingCart.Data.Repository;
using TGA.ECommerceApp.ShoppingCart.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);


var productDbConnectionStr = builder.Configuration.GetConnectionString("CartDbConnection");
builder.Services.AddDbContextPool<CartDbContext>(options =>
{
    options.UseMySql(productDbConnectionStr, ServerVersion.AutoDetect(productDbConnectionStr));
});

//automapper
IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Add services to the container.
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<BackendApiAuthenticationHttpClientHandler>();

builder.Services.AddHttpClient("Product", c => c.BaseAddress = new Uri(builder.Configuration["ServiceUrls:ProductAPI"]))
    .AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>()
    .AddTransientHttpErrorPolicy(policy => policy.CircuitBreakerAsync(3, TimeSpan.FromMilliseconds(120000)));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddAppAuthentication();

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
    var _db = scope.ServiceProvider.GetRequiredService<CartDbContext>();
    if (_db.Database.GetPendingMigrations().Any())
        _db.Database.Migrate();
}