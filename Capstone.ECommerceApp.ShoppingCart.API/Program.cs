using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Polly;
using System.Reflection;
using Capstone.ECommerceApp.ShoppingCart.API.Extensions;
using Capstone.ECommerceApp.ShoppingCart.API.Utility;
using Capstone.ECommerceApp.ShoppingCart.Application;
using Capstone.ECommerceApp.ShoppingCart.Application.Interfaces;
using Capstone.ECommerceApp.ShoppingCart.Application.Services;
using Capstone.ECommerceApp.ShoppingCart.Data.Context;
using Capstone.ECommerceApp.ShoppingCart.Data.Repository;
using Capstone.ECommerceApp.ShoppingCart.Domain.Interfaces;

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
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Cart API", Version = "v1" });
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

builder.AddAppAuthentication();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cart API v1");
    });
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