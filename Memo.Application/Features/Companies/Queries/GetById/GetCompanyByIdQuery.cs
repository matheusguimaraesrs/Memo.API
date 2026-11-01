using Memo.Application.Common.CQRS;
using Memo.Application.Features.Companies.Queries.Response;

namespace Memo.Application.Features.Companies.Queries.GetById;

public sealed record GetCompanyByIdQuery(Guid Id) : IRequest<CompanyResponse>;