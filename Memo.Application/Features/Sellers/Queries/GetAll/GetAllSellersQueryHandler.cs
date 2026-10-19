using Memo.Application.Common.CQRS;
using Memo.Application.Features.Sellers.Queries.Response;
using Memo.Domain.Entities;
using Memo.Domain.IRepositories;

namespace Memo.Application.Features.Sellers.Queries.GetAll;

internal sealed class GetAllSellersQueryHandler(ISellerRepository repository)
    : IRequestHandler<GetAllSellersQuery, IEnumerable<SellerResponse>>
{
    public async Task<IEnumerable<SellerResponse>> HandleAsync(GetAllSellersQuery request, CancellationToken ct = default)
    {
        IEnumerable<Seller> sellers = await repository.GetAllAsync();

        return sellers.Select(seller => new SellerResponse(
            seller.Id,
            seller.Name,
            seller.Login));
    }
}
