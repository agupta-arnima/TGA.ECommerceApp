using Capstone.ECommerceApp.Product.Application.Dto;

namespace Capstone.ECommerceApp.Product.Application.Interfaces;

public interface IProductService
{
    IEnumerable<ProductDto> GetProducts();
    ProductDto GetProductById(int id);
    bool DeleteProduct(int id);
    ProductDto AddProduct(ProductDto product);
    ProductDto UpdateProduct(ProductDto product);
    SupplierDto SupplierById(int id);
    SupplierDto AddSupplier(SupplierDto supplier);
    ProductDto UpdateSupplier(SupplierDto supplier);
    ProductDto DeleteSupplier(int id);
    CategoryDto CategoryById(int id);
    CategoryDto AddCategory(CategoryDto category);
    CategoryDto UpdateCategory(CategoryDto category);

    Task<bool> ReserveInventory(OrderHeaderDto order);
    Task<bool> ReleaseInventory(OrderHeaderDto order);


}
