using Memo.Application.Common.CQRS;

namespace Memo.Application.Features.Sellers.Commands.Update;

public record UpdateSellerCommand(Guid Id, string Name, string Login, string Password) : IRequest<Guid>;