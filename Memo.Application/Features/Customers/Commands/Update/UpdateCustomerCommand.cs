using Memo.Application.Common.CQRS;

namespace Memo.Application.Features.Customers.Commands.Update;

public sealed record UpdateCustomerCommand(Guid Id, string Name, AddressInput Address) : IRequest<Guid>;