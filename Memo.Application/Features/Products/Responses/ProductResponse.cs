namespace Memo.Application.Features.Products.Responses;

public sealed record ProductResponse(string Name, string Description, decimal UnitPrice, Guid CompanyId);