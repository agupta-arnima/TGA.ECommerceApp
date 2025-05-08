using Capstone.ECommerceApp.Domain.Core.Bus;
using Capstone.ECommerceApp.Infra.Bus;
using Capstone.ECommerceApp.Infra.Common;
using Capstone.ECommerceApp.Order.Application.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Capstone.ECommerceApp.Order.API.Messaging;

public class RabbitMqConsumer : IMessageConsumer, IDisposable
{
    private readonly ILogger<RabbitMqConsumer> _logger;
    private readonly RabbitMQSetting _rabbitMqSetting;
    private IConnection _connection;
    private IChannel _channel;
    private readonly IServiceProvider _serviceProvider;

    public RabbitMqConsumer(ILogger<RabbitMqConsumer> logger,
                            IOptions<RabbitMQSetting> rabbitMqSetting,
                            IServiceProvider serviceProvider)
    {
        _logger = logger;
        _rabbitMqSetting = rabbitMqSetting.Value;
        _serviceProvider = serviceProvider;

        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqSetting.HostName,
            UserName = _rabbitMqSetting.UserName,
            Password = _rabbitMqSetting.Password
        };
        _connection = factory.CreateConnectionAsync().Result;
        _channel = _connection.CreateChannelAsync().Result;
    }

    public async Task StartConsuming(string queueName, CancellationToken cancellationToken)
    {
        await _channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            _logger.LogInformation("Received message {0}", message);

            bool processedSuccessfully = false;
            try
            {
                var orderDeatils = JsonConvert.DeserializeObject<OrderMessage>(message);
                using (var scope = _serviceProvider.CreateScope())
                {
                    var token = string.Empty;
                    if (ea.BasicProperties.Headers.ContainsKey("Authorization"))
                    {
                        var tokenBytes = ea.BasicProperties.Headers["Authorization"] as byte[];
                        if (tokenBytes != null)
                        {
                            token = Encoding.UTF8.GetString(tokenBytes).Replace("Bearer ", "");
                        }
                    }
                    var orderProcessingService = scope.ServiceProvider.GetRequiredService<IOrderProcessingService>();
                    processedSuccessfully = await orderProcessingService.ProcessOrder(orderDeatils.order, token);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occurred while processing message from queue {queueName}: {ex}");
            }

            if (processedSuccessfully)
            {
                await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            else
            {
                await _channel.BasicRejectAsync(deliveryTag: ea.DeliveryTag, requeue: true);
            }
        };
        await _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);
    }

    public async void Dispose()
    {
        await _channel.CloseAsync();
        await _connection.CloseAsync();
    }
}
