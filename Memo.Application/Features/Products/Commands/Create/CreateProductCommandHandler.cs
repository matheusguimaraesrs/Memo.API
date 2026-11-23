using Memo.Application.Common.CQRS;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.Exceptions;
using Memo.Domain.IRepositories;

namespace Memo.Application.Features.Products.Commands.Create;

internal sealed class CreateProductCommandHandler(
    IProductRepository repository,
    ICompanyRepository companyRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateProductCommand, Guid>
{
    public async Task<Guid> HandleAsync(CreateProductCommand request, CancellationToken ct)
    {
        Company? company = await companyRepository.GetByIdAsync(request.CompanyId, ct);
        
        if (company is null)
            throw new EntityNotFoundException<Company>();

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