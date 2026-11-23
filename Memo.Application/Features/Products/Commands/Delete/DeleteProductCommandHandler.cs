using Memo.Application.Common.CQRS;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.Exceptions;
using Memo.Domain.IRepositories;

namespace Memo.Application.Features.Products.Commands.Delete;

internal sealed class DeleteProductCommandHandler(
    IProductRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteProductCommand, Guid>
{
    public async Task<Guid> HandleAsync(DeleteProductCommand request, CancellationToken ct)
    {
        Product? product = await repository.GetByIdAsync(request.id, ct);

        if (product is null)
            throw new EntityNotFoundException<Product>();

        repository.Delete(product);
        await unitOfWork.SaveChangesAsync(ct);
        return product.Id;
    }
}