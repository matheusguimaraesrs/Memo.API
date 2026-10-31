using Memo.Application.Common.CQRS;

namespace Memo.Application.Features.Companies.Commands.Delete;

public sealed record DeleteCompanyCommand(Guid Id) : IRequest<Guid>;