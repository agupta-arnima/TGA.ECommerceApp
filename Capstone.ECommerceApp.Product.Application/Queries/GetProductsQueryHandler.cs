using AutoMapper;
using Capstone.ECommerceApp.Product.Application.Dto;
using Capstone.ECommerceApp.Product.Domain.Interfaces;
using Capstone.ECommerceApp.Product.Domain.Models;
using MediatR;

namespace Capstone.ECommerceApp.Product.Application.Queries;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PagedResult<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductsQueryHandler(IProductRepository productRepository,
                                   IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetPagedProductsAsync(request.PageNumber, request.PageSize);
        return new PagedResult<ProductDto>
        {
            Items = _mapper.Map<List<ProductDto>>(products.Items),
            TotalCount = products.TotalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
