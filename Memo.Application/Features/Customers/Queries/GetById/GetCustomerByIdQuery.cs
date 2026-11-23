using Memo.Application.Common.CQRS;
using Memo.Application.Features.Customers.Responses;

namespace Memo.Application.Features.Customers.Queries.GetById;

public sealed record GetCustomerByIdQuery(Guid Id) : IRequest<CustomerResponse>;