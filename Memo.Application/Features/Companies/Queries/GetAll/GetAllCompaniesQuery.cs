using Memo.Application.Common.CQRS;
using Memo.Application.Features.Companies.Responses;
using Memo.Domain.Common;

namespace Memo.Application.Features.Companies.Queries.GetAll;

public sealed record GetAllCompaniesQuery(int Page, int PageSize) : IRequest<PagedList<CompanyResponse>>;