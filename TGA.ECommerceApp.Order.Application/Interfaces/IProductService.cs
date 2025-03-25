using TGA.ECommerceApp.Order.Application.Dto;

namespace TGA.ECommerceApp.Order.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
