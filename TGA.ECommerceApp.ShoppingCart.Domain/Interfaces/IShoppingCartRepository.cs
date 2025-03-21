using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGA.ECommerceApp.ShoppingCart.Domain.Models;

namespace TGA.ECommerceApp.ShoppingCart.Domain.Interfaces
{
    public interface ICartRepository
    {
        Task CartUpsert(CartHeader cartHeader, IEnumerable<CartDetails> cartDetails);
        IEnumerable<CartDetails> GetCartDetails(int cartHeaderId);
        Task<CartHeader> GetCartHeader(string userId);
        Task RemoveCart(int cartDetailId);
    }
}
