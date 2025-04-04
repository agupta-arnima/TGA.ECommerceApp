using Capstone.ECommerceApp.Order.Application.Dto;
using Capstone.ECommerceApp.Order.Application.Interfaces;

namespace Capstone.ECommerceApp.Order.Application.Services;

public class OrderProcessingService : IOrderProcessingService
{
    public Task<bool> ProcessOrder(OrderHeaderDto orderCreatedEvent, string token)
    {
        //Step 1
        //Reserve Inventory
        /*
         * if Reserve Inventory Failed :
         *  then Cancel the Order and Notification send to User
         * else
         * //Step 1
         * Process Payment
         * if Process Payment Failed
         *  then Refund the Payment and Canel the Order
         * else
         * //Step 3
         * InitiateShiiping
         * if Shipping is Failed
         *  then refund payment and cancel order
         *  else
         *  Send OrderConfirmation Notification
         */
        return Task.FromResult(true);
    }
}
