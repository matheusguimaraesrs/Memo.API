namespace Memo.Application.Features.Products.Responses;

public sealed record ProductResponse(Guid Id, string Name, string Description, decimal UnitPrice, Guid CompanyId);