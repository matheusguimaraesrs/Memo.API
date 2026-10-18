using Memo.Application.Common.CQRS;

namespace Memo.Application.Features.Sellers.Commands.Create;

public record CreateSellerCommand(string Name, string Login, string Password) : IRequest<Guid>;