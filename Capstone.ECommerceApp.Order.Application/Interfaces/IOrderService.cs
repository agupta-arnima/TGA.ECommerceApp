using Capstone.ECommerceApp.Order.Application.Dto;

namespace Capstone.ECommerceApp.Order.Application.Interfaces;

public interface IOrderService
{
    Task<OrderHeaderDto> CreateOrder(CartDto cartDto);
    Task<StripeRequestDto> CreateStripeSession(StripeRequestDto stripeRequestDto);
    Task<bool> CancelOrder(OrderHeaderDto cartDto);
}