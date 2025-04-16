using Capstone.ECommerceApp.ShoppingCart.Application.Dto;

namespace Capstone.ECommerceApp.ShoppingCart.Application.Interfaces
{
    public interface IProductService
    {
        Task<ProductDto> GetProductsById(int id);
    }
}
