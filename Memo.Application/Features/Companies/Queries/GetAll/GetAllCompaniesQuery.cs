using Memo.Application.Common.CQRS;
using Memo.Application.Features.Companies.Queries.Response;
using Memo.Domain.Common;

namespace Memo.Application.Features.Companies.Queries.GetAll;

public sealed record GetAllCompaniesQuery(int Page = 1, int PageSize = 10) : IRequest<PagedList<CompanyResponse>>;