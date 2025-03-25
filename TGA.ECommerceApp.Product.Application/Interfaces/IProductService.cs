using TGA.ECommerceApp.Product.Application.Dto;

namespace TGA.ECommerceApp.Product.Application.Interfaces
{
    public interface IProductService
    {
        IEnumerable<ProductDto> GetProducts();
        ProductDto GetProductById(int id);
        bool DeleteProduct(int id);
        ProductDto AddProduct(ProductDto product);
    }
}
