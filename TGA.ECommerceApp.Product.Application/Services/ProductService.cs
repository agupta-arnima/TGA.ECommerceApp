using AutoMapper;
using TGA.ECommerceApp.Product.Application.Dto;
using TGA.ECommerceApp.Product.Application.Interfaces;
using TGA.ECommerceApp.Product.Domain.Interfaces;
using TGA.ECommerceApp.Product.Domain.Models;

namespace TGA.ECommerceApp.Product.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository productRepository;
        private readonly IMapper mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            this.productRepository = productRepository;
            this.mapper = mapper;
        }

        public IEnumerable<ProductDto> GetProducts()
        {
            return mapper.Map<IEnumerable<ProductDto>>(productRepository.GetProducts());
        }

        public ProductDto GetProductById(int id)
        {
            return mapper.Map<ProductDto>(productRepository.GetProductById(id));
        }

        public bool DeleteProduct(int id) { 
           return productRepository.DeleteProduct(id);
        }

        public ProductDto AddProduct(ProductDto product)
        {
            Category category = new Category();
            Supplier supplier = new Supplier();

            if (product.CategotyId != null)
            {
                category = productRepository.GetCategoryById(product.CategotyId);
            }
            if (product.SupplierId != null)
            {
                supplier = productRepository.GetSupplierById(product.SupplierId);
            }

            var productInfo = new ProductInfo
            {
                CategoryId = category.Id,
                SupplierId = supplier.Id,
                Description = product.Description,
                Name = product.Name,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                Stock = product.Stock
            };

            return mapper.Map<ProductDto>(productRepository.CreateProduct(productInfo));
        }

        public ProductDto UpdateProduct(ProductDto product)
        {
            throw new NotImplementedException();
        }

        public SupplierDto SupplierById(int id)
        {
            throw new NotImplementedException();
        }

        public SupplierDto AddSupplier(SupplierDto supplierDto)
        {
            var supplier = mapper.Map<Supplier>(supplierDto);
            var createdSupplier = productRepository.CreateSupplier(supplier);
            return mapper.Map<SupplierDto>(createdSupplier);
        }

        public ProductDto UpdateSupplier(SupplierDto supplier)
        {
            throw new NotImplementedException();
        }

        public ProductDto DeleteSupplier(int id)
        {
            throw new NotImplementedException();
        }

        public CategoryDto CategoryById(int id)
        {
            throw new NotImplementedException();
        }

        public CategoryDto AddCategory(CategoryDto categoryDto)
        {
            var category = mapper.Map<Category>(categoryDto);
            var createdCategory = productRepository.CreateCategory(category);
            return mapper.Map<CategoryDto>(createdCategory);
        }

        public CategoryDto UpdateCategory(CategoryDto category)
        {
            throw new NotImplementedException();
        }
    }
}