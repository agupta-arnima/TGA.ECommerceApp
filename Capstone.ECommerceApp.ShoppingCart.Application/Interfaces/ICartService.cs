using Capstone.ECommerceApp.Infra.Common;

namespace Capstone.ECommerceApp.ShoppingCart.Application.Interfaces
{
    public interface ICartService
    {
        Task CartUpsert(CartDto cartDto);
        Task<CartDto> GetCart(string userId);
        Task<bool> ClearCart(string userId);
    }
}