using Memo.Application.Common.CQRS;
using Memo.Application.Features.Sellers.Queries.Response;
using Memo.Domain.Entities;
using Memo.Domain.Exceptions;
using Memo.Domain.IRepositories;

namespace Memo.Application.Features.Sellers.Queries.GetById;

internal sealed class GetSellerByIdQueryHandler(ISellerRepository repository) 
    : IRequestHandler<GetSellerByIdQuery, SellerResponse>
{
    public async Task<SellerResponse> HandleAsync(GetSellerByIdQuery request, CancellationToken ct = default)
    {
        Seller? seller = await repository.GetByIdAsync(request.Id);

        if (seller is null)
            throw new EntityNotFoundException<Seller>();

        return new SellerResponse(seller.Id, seller.Name, seller.Login);
    }
}