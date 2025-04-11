using Capstone.ECommerceApp.Order.Application.Dto;

namespace Capstone.ECommerceApp.Order.Application.Interfaces;

public interface IInventoryService
{
    Task<bool> ReleaseInventory(OrderHeaderDto order, string token);
    Task<bool> ReserveInventory(OrderHeaderDto order, string token);
}
