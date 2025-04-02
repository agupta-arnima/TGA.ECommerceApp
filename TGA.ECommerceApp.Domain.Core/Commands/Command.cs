
using TGA.ECommerceApp.Domain.Core.Events;

namespace TGA.ECommerceApp.Domain.Core.Commands
{
    public abstract class Command : Message
    {
        public DateTime Timestamp { get; protected set; } //Basic property is this command has to send some time.
        protected Command()
        {
            Timestamp = DateTime.Now;
        }
    } 
}
