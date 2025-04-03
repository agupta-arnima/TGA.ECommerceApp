using TGA.ECommerceApp.Product.Domain.Models;

namespace TGA.ECommerceApp.Product.Domain.Interfaces;

public interface IProductRepository
{
    IEnumerable<ProductInfo> GetProducts();
    ProductInfo GetProductById(int id);
    ProductInfo CreateProduct(ProductInfo productInfo);
    Task UpdateProduct(ProductInfo product);
    bool DeleteProduct(int id);
    Category GetCategoryById(int id);
    Category CreateCategory(Category category);
    Category UpdateCategory(Category category);
    Category DeleteCategory(int id);
    Supplier GetSupplierById(int id);
    Supplier CreateSupplier(Supplier supplier);
    Supplier UpdateSupplier(Supplier supplier);
    Supplier DeleteSupplier(int id);

}
