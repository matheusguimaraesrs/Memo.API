using Memo.Application.Common.CQRS;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.Exceptions;
using Memo.Domain.IRepositories;

namespace Memo.Application.Features.Sellers.Commands.Update;

internal sealed class UpdateSellerCommandHandler(
    ISellerRepository repository,
    IUnitOfWork unitOfWork) 
    : IRequestHandler<UpdateSellerCommand, Guid>
{
    public async Task<Guid> HandleAsync(UpdateSellerCommand request, CancellationToken ct = default)
    {
        Seller? seller = await repository.GetByIdAsync(request.Id);
        
        if (seller is null)
            throw new EntityNotFoundException<Seller>();
        
        seller.Update(request.Name, request.Login, request.Password);
        repository.Update(seller);
        await unitOfWork.SaveChangesAsync(ct);

        return seller.Id;
    }
}
