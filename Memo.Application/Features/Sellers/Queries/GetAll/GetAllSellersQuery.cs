using Memo.Application.Common.CQRS;
using Memo.Application.Features.Sellers.Responses;
using Memo.Domain.Common;

namespace Memo.Application.Features.Sellers.Queries.GetAll;

public sealed record GetAllSellersQuery(int Page, int PageSize) : IRequest<PagedList<SellerResponse>>;