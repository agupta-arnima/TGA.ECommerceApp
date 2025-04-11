using Capstone.ECommerceApp.Order.Application.Dto;
using Capstone.ECommerceApp.Order.Application.Interfaces;

namespace Capstone.ECommerceApp.Order.Application.Services;

public class OrderProcessingService : IOrderProcessingService
{
    private IInventoryService _inventoryService;
    private IProductService _productService;
    public OrderProcessingService(IInventoryService inventoryService,
                                  IProductService productService)
    {
        _inventoryService = inventoryService;
        _productService = productService;
    }


    public Task<bool> ProcessOrder(OrderHeaderDto orderCreatedEvent, string token)
    {
        //Step 1
        //Reserve Inventory
        var reserveInventory = _inventoryService.ReserveInventory(orderCreatedEvent, token).Result;
        if (!reserveInventory)
        {
           return Task.FromResult(false);
        }
        
        //Step 2
        //var process
        /*Process Payment
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
