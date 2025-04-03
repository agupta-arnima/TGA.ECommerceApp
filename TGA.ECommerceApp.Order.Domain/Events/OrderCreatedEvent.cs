using TGA.ECommerceApp.Domain.Core.Events;

namespace TGA.ECommerceApp.Order.Domain.Events;

public class OrderCreatedEvent<T>:Event
{
    public T order {  get; set; }
    public OrderCreatedEvent(T _order)
    {
        this.order = _order;
    }
}
