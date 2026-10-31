using Memo.Application.Common.CQRS;

namespace Memo.Application.Features.Companies.Commands.Create;

public sealed record CreateCompanyCommand(string Name, decimal Commission) : IRequest<Guid>;