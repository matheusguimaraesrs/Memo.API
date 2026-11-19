using Memo.Application.Common.CQRS;

namespace Memo.Application.Features.Customers.Commands.Create;

public sealed record CreateCustomerCommand(string Name, string Address) : IRequest<Guid>;