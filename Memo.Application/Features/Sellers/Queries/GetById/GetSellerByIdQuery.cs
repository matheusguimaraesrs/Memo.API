using Memo.Application.Common.CQRS;
using Memo.Application.Features.Sellers.Queries.Response;

namespace Memo.Application.Features.Sellers.Queries.GetById;

public sealed record GetSellerByIdQuery(Guid Id) : IRequest<SellerResponse>;