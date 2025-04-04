using Capstone.ECommerceApp.Order.Application.Dto;

namespace Capstone.ECommerceApp.Order.Application.Interfaces;

public interface IOrderProcessingService
{
    Task<bool> ProcessOrder(OrderHeaderDto orderCreatedEvent, string token);
}
