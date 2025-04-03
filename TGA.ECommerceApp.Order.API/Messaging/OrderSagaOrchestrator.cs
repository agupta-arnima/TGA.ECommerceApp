
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using TGA.ECommerceApp.Infra.Bus;

namespace TGA.ECommerceApp.Order.API.Messaging
{
    public class OrderSagaOrchestrator : BackgroundService
    {
        private readonly ILogger<OrderSagaOrchestrator> _logger;
        private readonly RabbitMQSetting _rabbitMqSetting;
        private IConnection _connection;
        private IChannel _channel;
        private IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public OrderSagaOrchestrator(ILogger<OrderSagaOrchestrator> logger,
                                        IOptions<RabbitMQSetting> rabbitMqSetting,
                                        IConfiguration configuration,
                                        IServiceProvider serviceProvider)
        {
            _logger = logger;
            _rabbitMqSetting = rabbitMqSetting.Value;
            _configuration = configuration;
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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var orderProcessingQueue = _configuration.GetValue<string>("ApiSettings:RabbitMQ:TopicAndQueueNames:OrderQueue");
            await StartConsuming(orderProcessingQueue, stoppingToken);
        }

        private async Task StartConsuming(string queueName, CancellationToken cancellationToken)
        {
            await _channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine("Received message {0}", message);

                bool processedSuccessfully = false;
                try
                {
                    //processedSuccessfully = await OnUserRegistrationReceived(message);
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

        public override async void Dispose()
        {
            await _channel.CloseAsync();
            await _connection.CloseAsync();
            base.Dispose();
        }
    }
}
