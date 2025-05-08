using Capstone.ECommerceApp.Domain.Core.Bus;
using Capstone.ECommerceApp.Order.API.Messaging;

namespace Capstone.ECommerceApp.Order.API
{
    public class MessageConsumerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public MessageConsumerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IMessageConsumer CreateConsumer(string brokerType)
        {
            return brokerType switch
            {
                "RabbitMQ" => _serviceProvider.GetRequiredService<RabbitMqConsumer>(),
                "EventHub" => _serviceProvider.GetRequiredService<EventHubConsumer>(),
                "ServiceBus" => _serviceProvider.GetRequiredService<ServiceBusConsumer>(),
                _ => throw new ArgumentException("Invalid broker type")
            };
        }
    }

}
