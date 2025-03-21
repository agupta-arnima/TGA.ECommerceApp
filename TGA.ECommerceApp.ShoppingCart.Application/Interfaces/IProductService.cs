using TGA.ECommerceApp.ShoppingCart.Application.Dto;

namespace TGA.ECommerceApp.ShoppingCart.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
