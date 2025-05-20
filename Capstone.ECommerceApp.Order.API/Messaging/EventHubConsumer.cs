using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using Capstone.ECommerceApp.Domain.Core.Bus;
using Capstone.ECommerceApp.Infra.Bus;
using Capstone.ECommerceApp.Infra.Common;
using Capstone.ECommerceApp.Order.Application.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace Capstone.ECommerceApp.Order.API.Messaging
{
    public class EventHubConsumer : IMessageConsumer
    {
        private readonly ILogger<EventHubConsumer> _logger;
        private readonly EventHubSetting _eventHubSetting;
        private readonly IServiceProvider _serviceProvider;
        private EventProcessorClient _processorClient;

        public EventHubConsumer(ILogger<EventHubConsumer> logger,
                                IOptions<EventHubSetting> eventHubSetting,
                                IServiceProvider serviceProvider)
        {
            _logger = logger;
            _eventHubSetting = eventHubSetting.Value;
            _serviceProvider = serviceProvider;

            var storageConnectionString = _eventHubSetting.StorageConnectionString;
            var blobContainerName = _eventHubSetting.BlobContainerName;
            var eventHubConnectionString = _eventHubSetting.ConnectionString;
            var eventHubName = _eventHubSetting.EventHubName;
            var consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

            var storageClient = new BlobContainerClient(storageConnectionString, blobContainerName);
            _processorClient = new EventProcessorClient(storageClient, consumerGroup, eventHubConnectionString, eventHubName);
        }

        private async Task ProcessEventHandler(ProcessEventArgs eventArgs)
        {
            var message = Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray());
            _logger.LogInformation("Received message {0}", message);

            bool processedSuccessfully = false;
            try
            {
                var orderDetails = JsonConvert.DeserializeObject<OrderMessage>(message);
                using (var scope = _serviceProvider.CreateScope())
                {
                    var token = string.Empty;
                    if (eventArgs.Data.Properties.ContainsKey("Authorization"))
                    {
                        var tokenstring = eventArgs.Data.Properties["Authorization"] as string;
                        if (tokenstring != null)
                        {
                            token = tokenstring.Replace("Bearer ", "");
                        }
                    }
                    var orderProcessingService = scope.ServiceProvider.GetRequiredService<IOrderProcessingService>();
                    processedSuccessfully = await orderProcessingService.ProcessOrder(orderDetails.order, token);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occurred while processing message: {ex}");
            }

            if (processedSuccessfully)
            {
                await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
            }
        }

        public async Task StartConsuming(string queueName, CancellationToken cancellationToken)
        {
            _processorClient.ProcessEventAsync += ProcessEventHandler;
            _processorClient.ProcessErrorAsync += ProcessErrorHandler;

            await _processorClient.StartProcessingAsync(cancellationToken);
        }

        private Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
        {
            _logger.LogError($"Error processing message: {eventArgs.Exception}");
            return Task.CompletedTask;
        }

        public async Task Stop()
        {
            await _processorClient.StopProcessingAsync();            
        }
    }
}