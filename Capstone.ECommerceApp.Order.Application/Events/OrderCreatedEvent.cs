using Capstone.ECommerceApp.Domain.Core.Events;
using Capstone.ECommerceApp.Infra.Common;

namespace Capstone.ECommerceApp.Order.Application.Events;

public class OrderCreatedEvent:Event
{
    public OrderHeaderDto order {  get; set; }
    public OrderCreatedEvent(OrderHeaderDto _order)
    {
        this.order = _order;
    }
}
