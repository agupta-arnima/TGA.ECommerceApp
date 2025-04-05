using TGA.ECommerceApp.Order.Application.Dto;

namespace TGA.ECommerceApp.Order.Application.Interfaces;

public interface IOrderProcessingService
{
    Task<bool> ProcessOrder(OrderHeaderDto orderCreatedEvent, string token);
}
