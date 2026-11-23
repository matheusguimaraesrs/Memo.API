using Memo.Application.Common.CQRS;

namespace Memo.Application.Features.Products.Commands.Delete;

public sealed record DeleteProductCommand(Guid Id) : IRequest<Guid>;