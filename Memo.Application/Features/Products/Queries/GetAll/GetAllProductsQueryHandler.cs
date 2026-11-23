using Memo.Application.Common.CQRS;
using Memo.Application.Features.Products.Responses;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.IRepositories;

namespace Memo.Application.Features.Products.Queries.GetAll;

internal sealed class GetAllProductsQueryHandler(IProductRepository repository)
    :IRequestHandler<GetAllProductsQuery, PagedList<ProductResponse>>
{
    public async Task<PagedList<ProductResponse>> HandleAsync(GetAllProductsQuery request, CancellationToken ct)
    {
        PagedList<Product> pagedList = await repository.GetPagedAsync(
            request.Page,
            request.PageSize,
            ct);

        IReadOnlyList<ProductResponse> products = pagedList.Items
            .Select(product => new ProductResponse(
                product.Id,
                product.Name,
                product.Description,
                product.UnitPrice,
                product.CompanyId))
            .ToList();

        return new PagedList<ProductResponse>(
            products,
            pagedList.Page,
            pagedList.PageSize,
            pagedList.TotalItems);
    }
}