using Capstone.ECommerceApp.Order.Application.Dto;
using Capstone.ECommerceApp.Order.Application.Interfaces;

namespace Capstone.ECommerceApp.Order.Application.Services;

public class PaymentService : IPaymentService
{
    public Task<bool> Process(OrderHeaderDto order, string token)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Refund(OrderHeaderDto order, string token)
    {
        throw new NotImplementedException();
    }
}
