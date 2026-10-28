using Memo.Application.Common.CQRS;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.Exceptions;
using Memo.Domain.IRepositories;

namespace Memo.Application.Features.Sellers.Commands.Delete;

internal sealed class DeleteSellerCommandHandler(
    ISellerRepository repository,
    IUnitOfWork unitOfWork) 
    : IRequestHandler<DeleteSellerCommand, Guid>
{
    public async Task<Guid> HandleAsync(DeleteSellerCommand request, CancellationToken ct = default)
    {
        Seller? seller = await repository.GetByIdAsync(request.Id);

        if (seller is null)
            throw new EntityNotFoundException<Seller>();

        repository.Delete(seller);
        await unitOfWork.SaveChangesAsync(ct);
        
        return seller.Id;
    }
}