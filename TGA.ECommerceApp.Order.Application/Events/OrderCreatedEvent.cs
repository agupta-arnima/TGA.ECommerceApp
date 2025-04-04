using TGA.ECommerceApp.Domain.Core.Events;
using TGA.ECommerceApp.Order.Application.Dto;

namespace TGA.ECommerceApp.Order.Application.Events;

public class OrderCreatedEvent:Event
{
    public OrderHeaderDto order {  get; set; }
    public OrderCreatedEvent(OrderHeaderDto _order)
    {
        this.order = _order;
    }
}
