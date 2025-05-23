using Capstone.ECommerceApp.Domain.Core.Bus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Capstone.ECommerceApp.Infra.Bus;

public static class EventBusFactory
{
    public static IEventBus CreateEventBus(MessageBrokerType brokerType, IConfiguration configuration)
    {
        switch (brokerType)
        {
            case MessageBrokerType.RabbitMQ:
                var rabbitMqSettings = configuration.GetSection("sackumar6:ApiSettings:RabbitMQ").Get<RabbitMQSetting>();
                return new RabbitMQBus(Options.Create(rabbitMqSettings));

            case MessageBrokerType.EventHub:
                var eventHubSettings = configuration.GetSection("sackumar6:ApiSettings:EventHub").Get<EventHubSetting>();
                return new EventHubBus(Options.Create(eventHubSettings));

            case MessageBrokerType.AzureServiceBus:
                throw new KeyNotFoundException("Not yet implemented");

            default:
                throw new ArgumentException("Invalid MessageBrokerType");
        }
    }
}

