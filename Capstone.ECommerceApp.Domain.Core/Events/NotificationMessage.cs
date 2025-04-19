namespace Capstone.ECommerceApp.Domain.Core.Events;


public class NotificationMessage<T>:Event
{
    public EventTypes EventType { get; set; }
    public required T Message { get; set; }

    public NotificationMessage() { }

    public NotificationMessage(EventTypes eventType, T message)
    {
        EventType = eventType;
        Message = message;
    }

    public override string ToString()
    {
        return $"EventType: {EventType}, Message: {Message}";
    }

}

