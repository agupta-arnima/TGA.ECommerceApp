using Capstone.ECommerceApp.Infra.Common;

namespace Capstone.ECommerceApp.ShoppingCart.Application.Interfaces
{
    public interface IProductService
    {
        Task<ProductDto> GetProductsById(int id);
        bool IsProductAvailable(int productId, int quantity);
    }
}
