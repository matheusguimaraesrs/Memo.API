using Memo.Application.Common.CQRS;
using Memo.Application.Features.Companies.Queries.Response;
using Memo.Domain.Entities;
using Memo.Domain.Exceptions;
using Memo.Domain.IRepositories;

namespace Memo.Application.Features.Companies.Queries.GetById;

internal sealed class GetCompanyByIdQueryHandler(ICompanyRepository repository)
    : IRequestHandler<GetCompanyByIdQuery, CompanyResponse>
{
    public async Task<CompanyResponse> HandleAsync(GetCompanyByIdQuery request, CancellationToken ct = default)
    {
        Company? company = await repository.GetByIdAsync(request.Id);

        if (company is null)
            throw new EntityNotFoundException<Company>();

        return new CompanyResponse(company.Id, company.Name, company.Commission);
    }
}