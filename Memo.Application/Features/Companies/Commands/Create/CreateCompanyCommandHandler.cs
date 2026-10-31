using Memo.Application.Common.CQRS;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.IRepositories;

namespace Memo.Application.Features.Companies.Commands.Create;

public class CreateCompanyCommandHandler(
    ICompanyRepository repository,
    IUnitOfWork unitOfWork) 
    : IRequestHandler<CreateCompanyCommand, Guid>
{
    public async Task<Guid> HandleAsync(CreateCompanyCommand request, CancellationToken ct = default)
    {
        Company company = new Company(request.Name, request.Commission);

        repository.Add(company);
        await unitOfWork.SaveChangesAsync(ct);
        
        return company.Id;
    }
}