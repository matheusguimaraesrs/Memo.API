using Memo.Application.Common.CQRS;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.IRepositories;

namespace Memo.Application.Features.Products.Commands.Create;

internal sealed class CreateProductCommandHandler(
    IProductRepository repository, 
    IUnitOfWork unitOfWork) : IRequestHandler<CreateProductCommand, Guid>
{
    public async Task<Guid> HandleAsync(CreateProductCommand request, CancellationToken ct)
    {
        Product product = new Product(
            request.Name, 
            request.Description, 
            request.UnitPrice, 
            request.CompanyId);

        repository.Add(product);
        await unitOfWork.SaveChangesAsync(ct);

        return product.Id;
    }
}