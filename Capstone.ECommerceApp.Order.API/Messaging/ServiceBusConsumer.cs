using Capstone.ECommerceApp.Domain.Core.Bus;

namespace Capstone.ECommerceApp.Order.API.Messaging;

public class ServiceBusConsumer : IMessageConsumer, IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public Task StartConsuming(string queueName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
