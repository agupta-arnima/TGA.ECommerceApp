namespace Capstone.ECommerceApp.Domain.Core.Events;


public class NotificationMessage<T>:Event
{
    public EventTypes EventType { get; set; }
    public T Message { get; set; }

    public NotificationMessage() { }

    public NotificationMessage(EventTypes eventType, T message)
    {
        EventType = eventType;
        Message = message;
    }
}

