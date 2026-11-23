using Memo.Application.Common.CQRS;
using Memo.Application.Features.Products.Responses;

namespace Memo.Application.Features.Products.Queries.GetById;

public sealed record GetProductByIdQuery(Guid Id) : IRequest<ProductResponse>;