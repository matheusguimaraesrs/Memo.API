using Memo.Application.Common.CQRS;
using Memo.Application.Features.Companies.Queries.Response;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.IRepositories;

namespace Memo.Application.Features.Companies.Queries.GetAll;

internal sealed class GetAllCompaniesQueryHandler(
    ICompanyRepository repository, 
    IUnitOfWork unitOfWork)
    : IRequestHandler<GetAllCompaniesQuery, PagedList<CompanyResponse>>
{
    public async Task<PagedList<CompanyResponse>> HandleAsync(GetAllCompaniesQuery request, CancellationToken ct = default)
    {
        PagedList<Company> pagedList = await repository.GetPagedAsync(
            request.Page, 
            request.PageSize, 
            ct);

        IReadOnlyList<CompanyResponse> companies = pagedList.Items
            .Select(company => new CompanyResponse(company.Id, company.Name, company.Commission))
            .ToList();

        return new PagedList<CompanyResponse>(
            companies,
            pagedList.Page,
            pagedList.PageSize,
            pagedList.TotalItems);
    }
}