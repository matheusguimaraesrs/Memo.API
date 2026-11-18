using Memo.Application.Common.CQRS;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.Exceptions;
using Memo.Domain.IRepositories;

namespace Memo.Application.Features.Companies.Commands.Update;

internal sealed class UpdateCompanyCommandHandler(
    ICompanyRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateCompanyCommand, Guid>
{
    public async Task<Guid> HandleAsync(UpdateCompanyCommand request, CancellationToken ct)
    {
        Company? company = await repository.GetByIdAsync(request.Id, ct);

        if (company is null)
            throw new EntityNotFoundException<Company>();
        
        company.Update(request.Name, request.Commission);
        repository.Update(company);
        await unitOfWork.SaveChangesAsync(ct);

        return company.Id;
    }
}