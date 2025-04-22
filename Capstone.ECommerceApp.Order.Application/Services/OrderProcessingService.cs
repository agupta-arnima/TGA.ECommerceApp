using Capstone.ECommerceApp.Domain.Core.Bus;
using Capstone.ECommerceApp.Infra.Common;
using Capstone.ECommerceApp.Order.Application.Interfaces;
using Capstone.ECommerceApp.Order.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Capstone.ECommerceApp.Order.Application.Services;

public class OrderProcessingService : IOrderProcessingService
{
    private IInventoryService _inventoryService;
    private IProductService _productService;
    private IPaymentService _paymentService;
    private IShippingService _shippingService;
    private IEventBus _messageBus;
    private IConfiguration _configuration;
    private IOrderService _orderService;
    private readonly ILogger<OrderProcessingService> _logger;

    public OrderProcessingService(IInventoryService inventoryService,
                                  IProductService productService,
                                  IPaymentService paymentService,
                                  IShippingService shippingService,
                                  IEventBus messageBus,
                                  IOrderService orderService,
                                  IConfiguration configuration,
                                  ILogger<OrderProcessingService> logger)
    {
        _inventoryService = inventoryService;
        _productService = productService;
        _paymentService = paymentService;
        _shippingService = shippingService;
        _messageBus = messageBus;
        _configuration = configuration;
        _orderService = orderService;
        _logger = logger;
    }

    public async Task<bool> ProcessOrder(OrderHeaderDto orderHeaderDto, string token)
    {
        if (!await ReserveInventory(orderHeaderDto, token))
        {
            _logger.LogError("Inventory Reserve failed! Cancelling Order Id: {0}", orderHeaderDto.OrderHeaderId);
            await CancelOrder(orderHeaderDto);
            return true;
        }

        if (!await ProcessPayment(orderHeaderDto, token))
        {
            _logger.LogError("Payment Processing failed! Cancelling Order Id: {0}", orderHeaderDto.OrderHeaderId);
            await ReleaseInventory(orderHeaderDto, token);
            await CancelOrder(orderHeaderDto);
            return true;
        }

        if (!await ShipOrder(orderHeaderDto, token))
        {
            _logger.LogError("Shipping order failed! Cancelling Order Id: {0}", orderHeaderDto.OrderHeaderId);
            await ReleaseInventory(orderHeaderDto, token);
            await RefundPayment(orderHeaderDto, token);
            await CancelOrder(orderHeaderDto);
            return true;
        }

        await CompleteOrder(orderHeaderDto);
        return true;
    }

    private async Task<bool> ReserveInventory(OrderHeaderDto orderHeaderDto, string token)
    {
        _logger.LogInformation("Reserving Inventory for Order Id: {0}!", orderHeaderDto.OrderHeaderId);
        var reserveInventory = await _inventoryService.ReserveInventory(orderHeaderDto, token);
        return reserveInventory;
    }

    private async Task<bool> ProcessPayment(OrderHeaderDto orderHeaderDto, string token)
    {
        _logger.LogInformation("Processing Payment for Order Id: {0}!", orderHeaderDto.OrderHeaderId);
        var paymentProcess = await _paymentService.Process(orderHeaderDto, token);
        return paymentProcess;
    }

    private async Task<bool> ShipOrder(OrderHeaderDto orderHeaderDto, string token)
    {
        _logger.LogInformation("Shipping Order for Order Id: {0}!", orderHeaderDto.OrderHeaderId);
        var shippingOrder = await _shippingService.ShippedOrder(orderHeaderDto, token);
        return shippingOrder;
    }

    private async Task ReleaseInventory(OrderHeaderDto orderHeaderDto, string token)
    {
        _logger.LogInformation("Releasing Inventory for Order Id: {0}!", orderHeaderDto.OrderHeaderId);
        await _inventoryService.ReleaseInventory(orderHeaderDto, token);
    }

    private async Task RefundPayment(OrderHeaderDto orderHeaderDto, string token)
    {
        _logger.LogInformation("Refunding Payment for Order Id: {0}!", orderHeaderDto.OrderHeaderId);
        await _paymentService.Refund(orderHeaderDto, token);
    }

    private async Task CancelOrder(OrderHeaderDto orderHeaderDto)
    {
        orderHeaderDto.Status = SD.Status_Canceled;
        await _orderService.UpdateOrderStatus(orderHeaderDto);
    }

    private async Task CompleteOrder(OrderHeaderDto orderHeaderDto)
    {
        _logger.LogInformation("Reserve inventory, processing payment, shipping order all are passed, completing Order Id - {0}!", orderHeaderDto.OrderHeaderId);
        orderHeaderDto.Status = SD.Status_Completed;
        await _orderService.UpdateOrderStatus(orderHeaderDto);
    }
}
