using Capstone.ECommerceApp.Infra.Common;

namespace Capstone.ECommerceApp.Order.Application.Interfaces;

public interface IShippingService
{
    Task<bool> ShippedOrder(OrderHeaderDto order, string token);
    Task<bool> CancleShipping(OrderHeaderDto order, string token);

}
