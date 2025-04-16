using Capstone.ECommerceApp.Domain.Core.Bus;
using Capstone.ECommerceApp.Order.Application.Dto;
using Capstone.ECommerceApp.Order.Application.Interfaces;
using Microsoft.VisualBasic;

namespace Capstone.ECommerceApp.Order.Application.Services;

public class OrderProcessingService : IOrderProcessingService
{
    private IInventoryService _inventoryService;
    private IProductService _productService;
    private IPaymentService _paymentService;
    private IShippingService _shippingService;
    private IEventBus _messageBus;
    public OrderProcessingService(IInventoryService inventoryService,
                                  IProductService productService,
                                  IPaymentService paymentService,
                                  IShippingService shippingService,
                                  IEventBus messageBus)
    {
        _inventoryService = inventoryService;
        _productService = productService;
        _paymentService = paymentService;
        _shippingService = shippingService;
        _messageBus = messageBus;
    }


    public async Task<bool> ProcessOrder(OrderHeaderDto orderCreatedEvent, string token)
    {
        //Step 1
        //Reserve Inventory
        var reserveInventory = _inventoryService.ReserveInventory(orderCreatedEvent, token).Result;
        if (!reserveInventory)
        {
            return false;
        }

        //Step 2
        //Payment Initiate
        var paymentProcess = _paymentService.Process(orderCreatedEvent, token).Result;
        if (!paymentProcess)
        {
            await _inventoryService.ReleaseInventory(orderCreatedEvent, token);
        }

        //Step 3
        // InitiateShiiping
        var shippingOrder = _shippingService.ShippedOrder(orderCreatedEvent, token).Result;
        if (!shippingOrder)
        {
            await _inventoryService.ReleaseInventory(orderCreatedEvent, token);
            await _paymentService.Refund(orderCreatedEvent, token);
        }

        //await _messageBus.PublishMessageAsync(orderCreatedEvent,
        //    configuration.GetValue<string>("ApiSettings:RabbitMQ:TopicAndQueueNames:UserRegistrationQueue"));
        return true;
    }
}
