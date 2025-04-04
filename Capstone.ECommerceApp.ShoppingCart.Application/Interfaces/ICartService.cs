using Capstone.ECommerceApp.ShoppingCart.Application.Dto;

namespace Capstone.ECommerceApp.ShoppingCart.Application.Interfaces
{
    public interface ICartService
    {
        Task CartUpsert(CartDto cartDto);
        Task<CartDto> GetCart(string userId);
        Task RemoveCart(int cartDetailId);
    }
}