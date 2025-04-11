using AutoMapper;
using Capstone.ECommerceApp.Domain.Core.Cache;
using Capstone.ECommerceApp.Infra.RedisCache;
using Capstone.ECommerceApp.ShoppingCart.API.Extensions;
using Capstone.ECommerceApp.ShoppingCart.API.Utility;
using Capstone.ECommerceApp.ShoppingCart.Application;
using Capstone.ECommerceApp.ShoppingCart.Application.Interfaces;
using Capstone.ECommerceApp.ShoppingCart.Application.Services;
using Microsoft.OpenApi.Models;
using Polly;
using StackExchange.Redis;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


//var productDbConnectionStr = builder.Configuration.GetConnectionString("CartDbConnection");
//builder.Services.AddDbContextPool<CartDbContext>(options =>
//{
//    options.UseMySql(productDbConnectionStr, ServerVersion.AutoDetect(productDbConnectionStr));
//});

//automapper
IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Add services to the container.
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartCacheService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<BackendApiAuthenticationHttpClientHandler>();

// Add Redis configuration
var redisConfiguration = builder.Configuration.GetSection("Redis")["ConnectionString"];
var redis = ConnectionMultiplexer.Connect(redisConfiguration);
//builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost"));
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();

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

//ApplyMigration();

app.Run();

//void ApplyMigration()
//{
//    using var scope = app.Services.CreateScope();
//    var _db = scope.ServiceProvider.GetRequiredService<CartDbContext>();
//    if (_db.Database.GetPendingMigrations().Any())
//        _db.Database.Migrate();
//}