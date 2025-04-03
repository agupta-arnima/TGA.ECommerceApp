using TGA.ECommerceApp.Order.Domain.Models;

namespace TGA.ECommerceApp.Order.Domain.Interfaces
{
    public interface IOrderRepository
    {
        Task<int> CreateOrder(OrderHeader orderHeader);
        Task<bool> CancelOrder(OrderHeader orderHeader);
        Task<OrderHeader> UpdateOrder(OrderHeader orderHeader);
        Task CreateStripeSession(int orderHeaderId, string stripeSessionId);
    }
}