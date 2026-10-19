using Memo.Application.Common.CQRS;
using Memo.Application.Features.Sellers.Queries.Response;

namespace Memo.Application.Features.Sellers.Queries.GetAll;

public sealed record GetAllSellersQuery : IRequest<IEnumerable<SellerResponse>>;