using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Capstone.ECommerceApp.Domain.Core.Bus;
using Capstone.ECommerceApp.Domain.Core.Commands;
using Capstone.ECommerceApp.Domain.Core.Events;

namespace Capstone.ECommerceApp.Infra.Bus;

public sealed class RabbitMQBus : IEventBus
{
    private readonly RabbitMQSetting _rabbitMqSetting;

    public RabbitMQBus(IOptions<RabbitMQSetting> rabbitMqSetting)
    {
        _rabbitMqSetting = rabbitMqSetting.Value;
    }
    public async Task PublishMessageAsync<T>(T @event, string queueName, string token = "") where T : Event
    {
        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqSetting.HostName,
            UserName = _rabbitMqSetting.UserName,
            Password = _rabbitMqSetting.Password
        };
        using (var connection = await factory.CreateConnectionAsync())  //Open the connection
        using (var channel = await connection.CreateChannelAsync())     //Open the channel
        {
            await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null); //Queue name and Routing key both are eventname
            var messageJson = System.Text.Json.JsonSerializer.Serialize(@event);
            var body = System.Text.Encoding.UTF8.GetBytes(messageJson);

            var properties = new BasicProperties
            {
                Headers = new Dictionary<string, object> {{ "Authorization", $"Bearer {token}"  } }
            };
            await channel.BasicPublishAsync(exchange: "", routingKey: queueName,true,basicProperties: properties, body: body);
            //In the case of Default Exchange, the binding key will be the same as the name of the queue.
            //So, the messages will also have the same routing-key as the Queue name.
        }
    }

    public Task SendCommandAsync<T>(T command) where T : Command
    {
        throw new NotImplementedException();
    }
}