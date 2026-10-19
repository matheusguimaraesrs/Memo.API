using Memo.Application.Common.CQRS;

namespace Memo.Application.Features.Sellers.Commands.Delete;

public sealed record DeleteSellerCommand(Guid Id) : IRequest<Guid>;