using Memo.Application.Common.CQRS;
using Memo.Application.Features.Products.Responses;
using Memo.Domain.Common;

namespace Memo.Application.Features.Products.Queries.GetAll;

public sealed record GetAllProductsQuery(int Page, int PageSize) : IRequest<PagedList<ProductResponse>>;