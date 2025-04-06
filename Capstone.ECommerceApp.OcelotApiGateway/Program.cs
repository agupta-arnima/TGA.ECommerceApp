using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using MMLib.SwaggerForOcelot.DependencyInjection;
using MMLib.SwaggerForOcelot.Middleware;
using Capstone.ECommerceApp.OcelotApiGateway.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddAppAuthentication();

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true); // Add Ocelot configuration file


builder.Services.AddOcelot(builder.Configuration); // Add Ocelot to the container
//builder.Services.AddSwaggerForOcelot(builder.Configuration);


var app = builder.Build();

//config swagger
//app.UseSwaggerForOcelotUI(opt =>
//{
//    opt.PathToSwaggerGenerator = "/swagger/docs";
//});

app.UseOcelot().GetAwaiter().GetResult(); // Add Ocelot to the pipeline
app.Run();