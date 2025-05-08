
using Capstone.ECommerceApp.Domain.Core.Bus;
using Capstone.ECommerceApp.Infra.Common;

namespace Capstone.ECommerceApp.Order.API.Messaging;

public class OrderSagaOrchestrator : BackgroundService
{

    private readonly IConfiguration _configuration;
    private readonly MessageConsumerFactory _consumerFactory;
    private IMessageConsumer _consumer;

    public OrderSagaOrchestrator(ILogger<OrderSagaOrchestrator> logger,
                                 IConfiguration configuration,
                                 IServiceProvider serviceProvider,
                                 MessageConsumerFactory consumerFactory)
    {
        _configuration = configuration;
        _consumerFactory = consumerFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var brokerType = _configuration.GetValue<string>("MessageBrokerType");
        var queueName = _configuration.GetValue<string>("ApiSettings:TopicAndQueueNames:OrderQueue");

        _consumer = _consumerFactory.CreateConsumer(brokerType);
        await _consumer.StartConsuming(queueName, stoppingToken);
    }

    public override async void Dispose()
    {
        base.Dispose();
    }
}
