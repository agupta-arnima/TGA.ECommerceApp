using Capstone.ECommerceApp.Domain.Core.Bus;
using Capstone.ECommerceApp.Domain.Core.Commands;
using Capstone.ECommerceApp.Domain.Core.Events;
using Microsoft.Extensions.Configuration;
    
namespace Capstone.ECommerceApp.Infra.Bus;

public class MessageBrokerService
{
    private readonly IEventBus _eventBus;

    public MessageBrokerService(IConfiguration configuration)
    {
        var brokerType = configuration.GetValue<MessageBrokerType>("MessageBrokerType");
        _eventBus = EventBusFactory.CreateEventBus(brokerType, configuration);
    }

    public async Task PublishEventAsync<T>(T @event, string queueName, string token = "") where T : Event
    {
        await _eventBus.PublishMessageAsync(@event, queueName, token);
    }

    public async Task SendCommandAsync<T>(T command) where T : Command
    {
        await _eventBus.SendCommandAsync(command);
    }

}
