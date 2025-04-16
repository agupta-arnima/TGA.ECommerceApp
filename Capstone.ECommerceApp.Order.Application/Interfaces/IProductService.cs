using Capstone.ECommerceApp.Order.Application.Dto;

namespace Capstone.ECommerceApp.Order.Application.Interfaces;

public interface IProductService
{
    Task<ProductDto> GetProductsById(int id);
}
