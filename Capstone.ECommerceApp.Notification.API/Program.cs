using Microsoft.EntityFrameworkCore;
using Capstone.ECommerceApp.Infra.Bus;
using Capstone.ECommerceApp.Notification.API.Messaging;
using Capstone.ECommerceApp.Notification.Application.Interfaces;
using Capstone.ECommerceApp.Notification.Application.Services;
using Capstone.ECommerceApp.Notification.Data.Context;
using Capstone.ECommerceApp.Notification.Data.Repository;
using Capstone.ECommerceApp.Notification.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var notificationDbConnectionStr = builder.Configuration.GetConnectionString("NotificationDbConnection");
builder.Services.AddDbContextPool<NotificationDbContext>(options =>
{
    options.UseMySql(notificationDbConnectionStr, ServerVersion.AutoDetect(notificationDbConnectionStr));
});

// Add services to the container.
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

//Rabbit MQ
builder.Services.Configure<RabbitMQSetting>(builder.Configuration.GetSection("ApiSettings:RabbitMQ"));

// Register the consumer service as a hosted service only
builder.Services.AddHostedService<UserRegistrationMessageConsumerService>();

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
    var _db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
    if (_db.Database.GetPendingMigrations().Any())
        _db.Database.Migrate();
}