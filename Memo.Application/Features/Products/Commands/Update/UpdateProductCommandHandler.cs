using Memo.Application.Common.CQRS;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.Exceptions;
using Memo.Domain.IRepositories;

namespace Memo.Application.Features.Products.Commands.Update;

internal sealed class UpdateProductCommandHandler(
    IProductRepository repository,
    ICompanyRepository companyRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateProductCommand, Guid>
{
    public async Task<Guid> HandleAsync(UpdateProductCommand request, CancellationToken ct)
    {
        Company? company = await companyRepository.GetByIdAsync(request.CompanyId, ct);
        Product? product = await repository.GetByIdAsync(request.Id, ct);

        if (company is null)
            throw new EntityNotFoundException<Company>();
        if (product is null)
            throw new EntityNotFoundException<Product>();
        
        product.Update(request.Name, request.Description, request.UnitPrice, request.CompanyId);
        repository.Update(product);
        await unitOfWork.SaveChangesAsync(ct);

        return product.Id;
    }
}