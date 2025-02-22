using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TGA.ECommerceApp.Product.Application;
using TGA.ECommerceApp.Product.Application.Interfaces;
using TGA.ECommerceApp.Product.Application.Services;
using TGA.ECommerceApp.Product.Data.Context;
using TGA.ECommerceApp.Product.Data.Repository;
using TGA.ECommerceApp.Product.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var productDbConnectionStr = builder.Configuration.GetConnectionString("ProductDbConnection");
builder.Services.AddDbContextPool<ProductDbContext>(options =>
{
    options.UseMySql(productDbConnectionStr, ServerVersion.AutoDetect(productDbConnectionStr));
});

//automapper
IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Add services to the container.
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

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

app.Run();