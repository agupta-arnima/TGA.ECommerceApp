using Azure.Messaging.ServiceBus;
using Capstone.ECommerceApp.Domain.Core.Bus;
using Capstone.ECommerceApp.Infra.Common;
using Capstone.ECommerceApp.Order.Application.Interfaces;
using Newtonsoft.Json;
using System.Text;
using ProcessErrorEventArgs = Azure.Messaging.ServiceBus.ProcessErrorEventArgs;

namespace Capstone.ECommerceApp.Order.API.Messaging
{
    public class ServiceBusConsumer : IMessageConsumer
    {
        private readonly ILogger<ServiceBusConsumer> _logger;
        public readonly string _serviceBusConnectionString;
        private readonly IConfiguration _configuration;
        private ServiceBusProcessor _orderProcessor;
        private readonly IServiceProvider _serviceProvider;

        public ServiceBusConsumer(ILogger<ServiceBusConsumer> logger, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _configuration = configuration;
            _serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            var client = new ServiceBusClient(_serviceBusConnectionString);
            _orderProcessor = client.CreateProcessor(_configuration.GetValue<string>("TopicAndQueueNames:OrderQueue"));
            _serviceProvider = serviceProvider;
        }

        public async Task StartConsuming(string queueName, CancellationToken cancellationToken)
        {
            _orderProcessor.ProcessMessageAsync += OnOrderReceived;
            _orderProcessor.ProcessErrorAsync += ErrorHandler;

            await _orderProcessor.StartProcessingAsync(cancellationToken); //Begin processing the messages
        }

        private async Task OnOrderReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            _logger.LogInformation("Received message {0}", body);

            bool processedSuccessfully = false;
            try
            {
                var orderDetails = JsonConvert.DeserializeObject<OrderMessage>(body);
                using (var scope = _serviceProvider.CreateScope())
                {
                    var token = string.Empty;
                    if (args.Message.ApplicationProperties.ContainsKey("Authorization"))
                    {
                        var tokenstring = args.Message.ApplicationProperties["Authorization"] as string;
                        if (tokenstring != null)
                        {
                            token = tokenstring.Replace("Bearer ", "");
                        }
                    }
                    var orderProcessingService = scope.ServiceProvider.GetRequiredService<IOrderProcessingService>();
                    processedSuccessfully = await orderProcessingService.ProcessOrder(orderDetails.order, token);
                    if (processedSuccessfully)
                    {
                        await args.CompleteMessageAsync(args.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                await args.AbandonMessageAsync(args.Message);
                _logger.LogError($"Exception occurred while processing message: {ex}");
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs eventArgs)
        {
            _logger.LogError($"Error processing message: {eventArgs.Exception}");
            return Task.CompletedTask;
        }

        public async Task Stop()
        {
            await _orderProcessor.StopProcessingAsync();
            await _orderProcessor.DisposeAsync();
        }
    }
}