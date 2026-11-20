using Memo.Application.Common.CQRS;
using Memo.Application.Features.Customers.Response;
using Memo.Domain.Common;

namespace Memo.Application.Features.Customers.Queries.GetAll;

public sealed record GetAllCustomersQuery(int Page = 1, int PageSize = 10) : IRequest<PagedList<CustomerResponse>>;