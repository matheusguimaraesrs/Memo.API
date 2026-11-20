using Memo.Domain.Enums;

namespace Memo.Application.Features.Customers.Response;

public sealed record AddressResponse(
    string PostalCode,
    string Number,
    string Street,
    string Neighborhood,
    string City,
    BrazilianState State);