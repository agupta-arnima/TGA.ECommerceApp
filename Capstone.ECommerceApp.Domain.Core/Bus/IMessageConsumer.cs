namespace Capstone.ECommerceApp.Domain.Core.Bus;

public interface IMessageConsumer
{
    Task StartConsuming(string queueName, CancellationToken cancellationToken);
    Task Stop();
}
