using Capstone.ECommerceApp.Infra.Common;

namespace Capstone.ECommerceApp.Order.Application.Interfaces;

public interface IPaymentService
{
    Task<bool> Process(OrderHeaderDto order, string token);
    Task<bool> Refund(OrderHeaderDto order, string token);
}
