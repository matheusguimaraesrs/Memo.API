using Memo.Application.Common.CQRS;
using Memo.Application.Features.Sellers.Queries.Response;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.IRepositories;

namespace Memo.Application.Features.Sellers.Queries.GetAll;

internal sealed class GetAllSellersQueryHandler(ISellerRepository repository)
    : IRequestHandler<GetAllSellersQuery, PagedList<SellerResponse>>
{
    public async Task<PagedList<SellerResponse>> HandleAsync(GetAllSellersQuery request, CancellationToken ct = default)
    {
        PagedList<Seller> result = await repository.GetPagedAsync(
            request.Page,
            request.PageSize,
            ct);

        IReadOnlyList<SellerResponse> sellers = result.Items
            .Select(seller => new SellerResponse(
                seller.Id, seller.Name, seller.Login))
            .ToList();

        return new PagedList<SellerResponse>(
            sellers,
            result.Page,
            result.PageSize,
            result.TotalItems);
    }
}
