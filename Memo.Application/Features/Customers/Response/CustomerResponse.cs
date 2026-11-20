namespace Memo.Application.Features.Customers.Response;

public sealed record CustomerResponse(Guid Id, string Name, AddressResponse Address);