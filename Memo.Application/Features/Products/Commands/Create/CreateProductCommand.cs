using Memo.Application.Common.CQRS;

namespace Memo.Application.Features.Products.Commands.Create;

public sealed record CreateProductCommand(
    string Name, 
    string Description, 
    decimal UnitPrice, 
    Guid CompanyId) : IRequest<Guid>;