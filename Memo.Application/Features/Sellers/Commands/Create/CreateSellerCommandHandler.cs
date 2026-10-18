using System.Reflection.Metadata;
using Memo.Application.Common.CQRS;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.IRepositories;

namespace Memo.Application.Features.Sellers.Commands.Create;

internal sealed class CreateSellerCommandHandler(
    ISellerRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateSellerCommand, Guid>
{
    public async Task<Guid> HandleAsync(CreateSellerCommand request, CancellationToken ct = default)
    {
        Seller seller = new Seller(request.Name, request.Login, request.Password);
        
        await repository.AddAsync(seller);
        await unitOfWork.SaveChangesAsync(ct);

        return seller.Id;
    }
}