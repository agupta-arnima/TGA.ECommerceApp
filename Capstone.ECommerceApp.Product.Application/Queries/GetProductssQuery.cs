using Capstone.ECommerceApp.Product.Application.Dto;
using Capstone.ECommerceApp.Product.Domain.Models;
using MediatR;

namespace Capstone.ECommerceApp.Product.Application.Queries;


public class GetProductsQuery : IRequest<PagedResult<ProductDto>>
{
    public int PageNumber { get; }
    public int PageSize { get; }

    public GetProductsQuery(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}

//public record GetProductsQuery() : IRequest<IEnumerable<ProductDto>>;
//public class GetProductssQueryHandler(IProductRepository productRepository, IMapper mapper)
//    : IRequestHandler<GetProductsQuery, IEnumerable<ProductDto>>
//{
//    public async Task<IEnumerable<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
//    {
//        return mapper.Map<IEnumerable<ProductDto>>(await productRepository.GetProducts());
//    }
//}
