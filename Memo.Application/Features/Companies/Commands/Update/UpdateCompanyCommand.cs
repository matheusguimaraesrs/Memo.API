using Memo.Application.Common.CQRS;

namespace Memo.Application.Features.Companies.Commands.Update;

public sealed record UpdateCompanyCommand(Guid Id, string Name, decimal Commission) : IRequest<Guid>;