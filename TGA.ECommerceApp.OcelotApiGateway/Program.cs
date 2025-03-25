using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using TGA.ECommerceApp.OcelotApiGateway.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddAppAuthentication();
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true); // Add Ocelot configuration file
builder.Services.AddOcelot(builder.Configuration); // Add Ocelot to the container

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.UseOcelot().GetAwaiter().GetResult(); // Add Ocelot to the pipeline
app.Run();