using TGA.ECommerceApp.Domain.Core.Commands;
using TGA.ECommerceApp.Domain.Core.Events;

namespace TGA.ECommerceApp.Domain.Core.Bus
{
    public interface IEventBus
    {
        Task SendCommandAsync<T>(T command) where T : Command;
        Task PublishMessageAsync<T>(T @event, string queueName, string token="") where T : Event;
        //event is reserved keyword
    }
}