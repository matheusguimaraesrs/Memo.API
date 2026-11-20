using Memo.Application.Common.CQRS;

namespace Memo.Application.Features.Customers.Commands.Delete;

public sealed record DeleteCustomerCommand(Guid Id) : IRequest<Guid>;