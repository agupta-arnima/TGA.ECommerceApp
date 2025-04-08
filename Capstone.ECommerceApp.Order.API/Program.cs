using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Capstone.ECommerceApp.Domain.Core.Bus;
using Capstone.ECommerceApp.Infra.Bus;
using Capstone.ECommerceApp.Order.API.Extensions;
using Capstone.ECommerceApp.Order.API.Messaging;
using Capstone.ECommerceApp.Order.API.Utility;
using Capstone.ECommerceApp.Order.Application;
using Capstone.ECommerceApp.Order.Application.Interfaces;
using Capstone.ECommerceApp.Order.Application.Services;
using Capstone.ECommerceApp.Order.Data.Context;
using Capstone.ECommerceApp.Order.Data.Repository;
using Capstone.ECommerceApp.Order.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var orderDbConnectionStr = builder.Configuration.GetConnectionString("OrderDbConnection");
builder.Services.AddDbContextPool<OrderDbContext>(options =>
{
    options.UseMySql(orderDbConnectionStr, ServerVersion.AutoDetect(orderDbConnectionStr));
});

//automapper
IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Add services to the container.
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddSingleton<IOrderProcessingService, OrderProcessingService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<BackendApiAuthenticationHttpClientHandler>();

builder.Services.AddHttpClient("Product", c => c.BaseAddress = new Uri(builder.Configuration["ServiceUrls:ProductAPI"]))
    .AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>();

//Rabbit MQ

builder.Services.Configure<RabbitMQSetting>(builder.Configuration.GetSection("ApiSettings:RabbitMQ"));
builder.Services.AddScoped(typeof(IEventBus), typeof(RabbitMQBus));
// Register the consumer service as a hosted service only
builder.Services.AddHostedService<OrderSagaOrchestrator>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.AddAppAuthentication();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Order API", Version = "v1" });
    //options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
    
    options.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[]{}
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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order API v1");
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
    var _db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    if (_db.Database.GetPendingMigrations().Any())
        _db.Database.Migrate();
}