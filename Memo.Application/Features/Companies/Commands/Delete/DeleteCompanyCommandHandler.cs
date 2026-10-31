using Memo.Application.Common.CQRS;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.Exceptions;
using Memo.Domain.IRepositories;

namespace Memo.Application.Features.Companies.Commands.Delete;

internal sealed class DeleteCompanyCommandHandler(
    ICompanyRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteCompanyCommand, Guid>
{
    public async Task<Guid> HandleAsync(DeleteCompanyCommand request, CancellationToken ct = default)
    {
        Company? company = await repository.GetByIdAsync(request.Id);

        if (company is null)
            throw new EntityNotFoundException<Company>();

        repository.Delete(company);
        await unitOfWork.SaveChangesAsync(ct);

        return company.Id;
    }
}