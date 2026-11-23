using Memo.Application.Common.CQRS;

namespace Memo.Application.Features.Products.Commands.Update;

public sealed record UpdateProductCommand(
    Guid Id, 
    string Name, 
    string? Description, 
    decimal UnitPrice, 
    Guid CompanyId) : IRequest<Guid>;