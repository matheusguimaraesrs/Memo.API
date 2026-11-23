namespace Memo.Application.Features.Customers.Responses;

public sealed record CustomerResponse(Guid Id, string Name, AddressResponse Address);