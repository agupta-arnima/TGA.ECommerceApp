using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Capstone.ECommerceApp.Domain.Core.Bus;
using Capstone.ECommerceApp.Domain.Core.Commands;
using Capstone.ECommerceApp.Domain.Core.Events;
using Microsoft.Extensions.Options;

namespace Capstone.ECommerceApp.Infra.Bus;

public sealed class EventHubBus : IEventBus
{
    private readonly EventHubSetting _eventHubSetting;

    public EventHubBus(IOptions<EventHubSetting> eventHubSetting)
    {
        _eventHubSetting = eventHubSetting.Value;
    }

    public async Task PublishMessageAsync<T>(T @event, string queueName, string token = "") where T : Event
    {
        await using (var producerClient = new EventHubProducerClient(_eventHubSetting.ConnectionString, queueName))
        {
            using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();
            var messageJson = System.Text.Json.JsonSerializer.Serialize(@event);
            var eventData = new EventData(messageJson);

            eventData.Properties["Authorization"] = $"Bearer {token}";

            eventBatch.TryAdd(eventData);
            await producerClient.SendAsync(eventBatch);
        }
    }

    public Task SendCommandAsync<T>(T command) where T : Command
    {
        throw new NotImplementedException();
    }
}
