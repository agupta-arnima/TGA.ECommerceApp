using TGA.ECommerceApp.Order.Domain.Models;

namespace TGA.ECommerceApp.Order.Domain.Interfaces
{
    public interface IOrderRepository
    {
        Task<int> CreateOrder(OrderHeader orderHeader);
        Task CreateStripeSession(int orderHeaderId, string stripeSessionId);
    }
}