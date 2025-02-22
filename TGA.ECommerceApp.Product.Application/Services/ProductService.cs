using AutoMapper;
using TGA.ECommerceApp.Product.Application.Dto;
using TGA.ECommerceApp.Product.Application.Interfaces;
using TGA.ECommerceApp.Product.Domain.Interfaces;

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
    }
}