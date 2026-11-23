using Memo.Application.Common.CQRS;
using Memo.Application.Features.Customers.Responses;
using Memo.Domain.Common;

namespace Memo.Application.Features.Customers.Queries.GetAll;

public sealed record GetAllCustomersQuery(int Page, int PageSize) : IRequest<PagedList<CustomerResponse>>;