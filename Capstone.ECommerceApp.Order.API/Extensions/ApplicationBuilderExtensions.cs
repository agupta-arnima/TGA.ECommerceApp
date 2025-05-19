using Capstone.ECommerceApp.Domain.Core.Bus;
using Capstone.ECommerceApp.Order.API.Messaging;

namespace Capstone.ECommerceApp.Order.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        private static IMessageConsumer ServiceBusConsumer { get; set; }
        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
        {
            ServiceBusConsumer = app.ApplicationServices.GetService<ServiceBusConsumer>(); //Need to register the service in Dependency Injection
            var hostApplicationLifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>(); //Application lifetime is needed to start and stop the service
            //hostApplicationLifetime.ApplicationStarted.Register(() => ServiceBusConsumer.Start());
            //hostApplicationLifetime.ApplicationStopped.Register(() => ServiceBusConsumer.Stop());
            hostApplicationLifetime.ApplicationStarted.Register(OnStart);
            hostApplicationLifetime.ApplicationStopped.Register(OnStop);
            return app;  //Don't want to hold up the pipeline
        }
        private static void OnStart()
        {
            //TBD
            //ServiceBusConsumer.StartConsuming();
        }
        private static void OnStop()
        {
            ServiceBusConsumer.Stop();
        }
    }
}
