using Azure.Messaging.ServiceBus;
using Capstone.ECommerceApp.Domain.Core.Bus;
using Capstone.ECommerceApp.Domain.Core.Commands;
using Capstone.ECommerceApp.Domain.Core.Events;
using System.Text;

namespace Capstone.ECommerceApp.Infra.Bus.MessageBus
{
    public sealed class ServiceBus : IEventBus
    {
        private readonly string _messageBusConnectionString;

        public ServiceBus(string messageBusConnectionString)
        {
            _messageBusConnectionString = messageBusConnectionString;
        }

        public async Task PublishMessageAsync<T>(T @event, string topic_queue_name, string token = "") where T : Event
        {
            await using var client = new ServiceBusClient(_messageBusConnectionString);

            ServiceBusSender sender = client.CreateSender(topic_queue_name);

            var jsonMessage = System.Text.Json.JsonSerializer.Serialize(@event);

            ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString()
            };

            await sender.SendMessageAsync(finalMessage);
            await client.DisposeAsync();
        }

        public Task SendCommandAsync<T>(T command) where T : Command
        {
            throw new NotImplementedException();
        }
    }
}