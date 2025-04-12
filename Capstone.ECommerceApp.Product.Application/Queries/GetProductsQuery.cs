using AutoMapper;
using Capstone.ECommerceApp.Product.Application.Dto;
using Capstone.ECommerceApp.Product.Domain.Interfaces;
using MediatR;

namespace Capstone.ECommerceApp.Product.Application.Queries
{
    public record GetProductsQuery() : IRequest<IEnumerable<ProductDto>>;

    public class GetProductssQueryHandler(IProductRepository productRepository, IMapper mapper)
        : IRequestHandler<GetProductsQuery, IEnumerable<ProductDto>>
    {
        public async Task<IEnumerable<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            return mapper.Map<IEnumerable<ProductDto>>(await productRepository.GetProducts());
        }
    }
}