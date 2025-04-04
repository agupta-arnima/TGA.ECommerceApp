using Capstone.ECommerceApp.ShoppingCart.Domain.Models;

namespace Capstone.ECommerceApp.ShoppingCart.Domain.Interfaces;

public interface ICartRepository
{
    Task CartUpsert(CartHeader cartHeader, IEnumerable<CartDetails> cartDetails);
    IEnumerable<CartDetails> GetCartDetails(int cartHeaderId);
    Task<CartHeader> GetCartHeader(string userId);
    Task RemoveCart(int cartDetailId);
}
