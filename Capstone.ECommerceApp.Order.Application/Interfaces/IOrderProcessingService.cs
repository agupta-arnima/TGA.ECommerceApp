using Capstone.ECommerceApp.Infra.Common;

namespace Capstone.ECommerceApp.Order.Application.Interfaces;

public interface IOrderProcessingService
{
    Task<bool> ProcessOrder(OrderHeaderDto orderCreatedEvent, string token);
}
