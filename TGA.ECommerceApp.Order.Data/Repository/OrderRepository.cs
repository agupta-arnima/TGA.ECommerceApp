using Microsoft.EntityFrameworkCore;
using TGA.ECommerceApp.Order.Data.Context;
using TGA.ECommerceApp.Order.Domain.Interfaces;
using TGA.ECommerceApp.Order.Domain.Models;

namespace TGA.ECommerceApp.Order.Data.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext orderDbContext;
        public OrderRepository(OrderDbContext orderDbContext)
        {
            this.orderDbContext = orderDbContext;
        }

        public async Task<int> CreateOrder(OrderHeader orderHeader)
        {
            OrderHeader orderHeaderFromDb = orderDbContext.OrderHeaders.Add(orderHeader).Entity;

            //_db.Entry(_mapper.Map<OrderHeader>(orderHeaderDto)).State = Microsoft.EntityFrameworkCore.EntityState.Added;
            //_db.Set<OrderHeader>().Add(_mapper.Map<OrderHeader>(orderHeaderDto));

            await orderDbContext.SaveChangesAsync();
            return orderHeaderFromDb.OrderHeaderId;
        }

        public async Task CreateStripeSession(int orderHeaderId, string stripeSessionId)
        {
            OrderHeader orderFromDb = await orderDbContext.OrderHeaders.FirstAsync(u => u.OrderHeaderId == orderHeaderId);
            orderFromDb.StripeSessionId = stripeSessionId;
            orderDbContext.SaveChanges();
        }

        public async Task<bool> CancelOrder(OrderHeader orderHeader)
        {
            OrderHeader order = await orderDbContext.OrderHeaders.FirstAsync(u=> u.OrderHeaderId == orderHeader.OrderHeaderId);
            order.Status = orderHeader.Status;
            orderDbContext.SaveChanges();
            return true;
        }

        public Task<OrderHeader> UpdateOrder(OrderHeader orderHeader)
        {
            throw new NotImplementedException();
        }
    }
}
