using Capstone.ECommerceApp.Domain.Core.Commands;
using Capstone.ECommerceApp.Domain.Core.Events;

namespace Capstone.ECommerceApp.Domain.Core.Bus
{
    public interface IEventBus
    {
        Task SendCommandAsync<T>(T command) where T : Command;
        Task PublishMessageAsync<T>(T @event, string queueName, string token="") where T : Event;
        //event is reserved keyword
    }
}