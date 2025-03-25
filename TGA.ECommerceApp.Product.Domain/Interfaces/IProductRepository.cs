using TGA.ECommerceApp.Product.Domain.Models;

namespace TGA.ECommerceApp.Product.Domain.Interfaces;

public interface IProductRepository
{
    IEnumerable<ProductInfo> GetProducts();
    ProductInfo GetProductById(int id);
    bool DeleteProduct(int id);
}
