using AutoMapper;
using Capstone.ECommerceApp.Order.API;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

//Define Polly retry Policy
var retryPolicy = HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                    retryAttempt)));

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
builder.Services.AddScoped<IInventoryService,InventoryService>();
builder.Services.AddScoped<IPaymentService,PaymentService>();
builder.Services.AddScoped<IShippingService, ShippingService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderProcessingService, OrderProcessingService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<BackendApiAuthenticationHttpClientHandler>();


// Add HttpClient services using the extension method
builder.Services.AddHttpClientService("Product",builder.Configuration["ServiceUrls:ProductAPI"],
                    sp => sp.GetRequiredService<BackendApiAuthenticationHttpClientHandler>(), retryPolicy);
builder.Services.AddHttpClientService("Inventory", builder.Configuration["ServiceUrls:InventoryAPI"],
                    sp => sp.GetRequiredService<BackendApiAuthenticationHttpClientHandler>(), retryPolicy);
builder.Services.AddHttpClientService("Payment", builder.Configuration["ServiceUrls:PaymentAPI"],
                    sp => sp.GetRequiredService<BackendApiAuthenticationHttpClientHandler>(), retryPolicy);



// Configure message broker settings
builder.Services.Configure<RabbitMQSetting>(builder.Configuration.GetSection("ApiSettings:RabbitMQ"));
builder.Services.Configure<EventHubSetting>(builder.Configuration.GetSection("ApiSettings:EventHub"));
//builder.Services.Configure<AzureServiceBusSetting>(builder.Configuration.GetSection("ApiSettings:AzureServiceBus"));

// Add the factory pattern for IEventBus
builder.Services.AddSingleton<IEventBus>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var brokerType = configuration.GetValue<MessageBrokerType>("MessageBrokerType");
    return EventBusFactory.CreateEventBus(brokerType, configuration);
});

// Register the consumer service as a hosted service only
builder.Services.AddSingleton<RabbitMqConsumer>();
builder.Services.AddSingleton<EventHubConsumer>();
builder.Services.AddSingleton<ServiceBusConsumer>();
builder.Services.AddSingleton<MessageConsumerFactory>();
builder.Services.AddHostedService<OrderSagaOrchestrator>();


builder.Services.AddSingleton<CheckoutsMetrics>();

builder.Services.AddOpenTelemetry()
    .WithMetrics(builder => builder
    //.AddConsoleExporter()
    .AddAspNetCoreInstrumentation()
    .AddHttpClientInstrumentation()
    .AddRuntimeInstrumentation()
    .AddPrometheusExporter()
    .AddMeter("capstone.checkouts.meter")
);

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

ConfigureLogging(builder.Configuration);
builder.Host.UseSerilog();

var app = builder.Build();

app.UseOpenTelemetryPrometheusScrapingEndpoint();

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

void ConfigureLogging(IConfigurationRoot configuration)
{
    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    //var configuration = new ConfigurationBuilder()
    //    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    //    .AddJsonFile(
    //        $"appsettings.{environment}.json", optional: true
    //    ).Build();

    Log.Logger = new LoggerConfiguration()
           .Enrich.FromLogContext()
           .Enrich.WithExceptionDetails()
           .WriteTo.Debug()
           .WriteTo.Console()
           .WriteTo.Elasticsearch(ConfigureElasticSink(configuration, environment))
           .Enrich.WithProperty("Environment", environment)
           .ReadFrom.Configuration(configuration)
           .CreateLogger();
}

ElasticsearchSinkOptions ConfigureElasticSink(IConfigurationRoot configuration, string environment)
{
    return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
    {
        AutoRegisterTemplate = true,
        IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{environment.ToLower()}-{DateTime.UtcNow:yyyy-MM}",
        NumberOfReplicas = 1,
        NumberOfShards = 1
    };
}