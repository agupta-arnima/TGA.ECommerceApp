using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Capstone.ECommerceApp.Infra.Bus;
using Capstone.ECommerceApp.Notification.Application.Interfaces;
using Capstone.ECommerceApp.Notification.Domain.Events;

namespace Capstone.ECommerceApp.Notification.API.Messaging
{
    public class UserRegistrationMessageConsumerService : BackgroundService
    {
        private readonly ILogger<UserRegistrationMessageConsumerService> logger;
        private readonly RabbitMQSetting rabbitMqSetting;
        private IConnection connection;
        private IChannel channel;
        private IConfiguration configuration;
        private readonly IServiceProvider serviceProvider;

        public UserRegistrationMessageConsumerService
            (
                IOptions<RabbitMQSetting> rabbitMqSetting, 
                IConfiguration configuration, 
                IServiceProvider serviceProvider, 
                ILogger<UserRegistrationMessageConsumerService> logger
            )
        {
            this.logger = logger;
            this.configuration = configuration;
            this.serviceProvider = serviceProvider;
            this.rabbitMqSetting = rabbitMqSetting.Value;
            var factory = new ConnectionFactory
            {
                HostName = this.rabbitMqSetting.HostName,
                UserName = this.rabbitMqSetting.UserName,
                Password = this.rabbitMqSetting.Password
            };
            connection = factory.CreateConnectionAsync().Result;
            channel = connection.CreateChannelAsync().Result;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var userRegistrationQueue = configuration.GetValue<string>("ApiSettings:RabbitMQ:TopicAndQueueNames:UserRegistrationQueue");
            await StartConsuming(userRegistrationQueue, stoppingToken);
        }

        private async Task StartConsuming(string queueName, CancellationToken cancellationToken)
        {
            await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine("Received message {0}", message);

                bool processedSuccessfully = false;
                try
                {
                    processedSuccessfully = await OnUserRegistrationReceived(message);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Exception occurred while processing message from queue {queueName}: {ex}");
                }

                if (processedSuccessfully)
                {
                    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                else
                {
                    await channel.BasicRejectAsync(deliveryTag: ea.DeliveryTag, requeue: true);
                }
            };
            await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);
        }

        private async Task<bool> OnUserRegistrationReceived(string message)
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var userRegistration = System.Text.Json.JsonSerializer.Deserialize<UserRegistrationEvent>(message);
                    var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                    await notificationService.RegisterUserEmailAndLog(userRegistration.Email);
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error processing message: {ex.Message}");
                return false;
            }
        }

        public override async void Dispose()
        {
            await channel.CloseAsync();
            await connection.CloseAsync();
            base.Dispose();
        }
    }
}