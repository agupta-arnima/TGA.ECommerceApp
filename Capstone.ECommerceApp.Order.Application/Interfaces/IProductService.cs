using Capstone.ECommerceApp.Infra.Common;

namespace Capstone.ECommerceApp.Order.Application.Interfaces;

public interface IProductService
{
    Task<ProductDto> GetProductsById(int id);
}
