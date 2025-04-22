using Capstone.ECommerceApp.Infra.Common;
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
