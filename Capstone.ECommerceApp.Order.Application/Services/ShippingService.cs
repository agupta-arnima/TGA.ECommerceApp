using Capstone.ECommerceApp.Infra.Common;
using Capstone.ECommerceApp.Order.Application.Interfaces;

namespace Capstone.ECommerceApp.Order.Application.Services;

public class ShippingService : IShippingService
{
    public Task<bool> CancleShipping(OrderHeaderDto order, string token)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ShippedOrder(OrderHeaderDto order, string token)
    {
        return Task.FromResult(true);
    }
}
