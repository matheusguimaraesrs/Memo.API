using Memo.Application.Common.CQRS;

namespace Memo.Application.Features.Customers.Commands.Create;

public sealed record CreateCustomerCommand(string Name, AddressInput Address) : IRequest<Guid>;