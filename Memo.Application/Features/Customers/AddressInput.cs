using Memo.Domain.Enums;

namespace Memo.Application.Features.Customers;

public sealed record AddressInput(string PostalCode,
    string Number,
    string Street,
    string Neighborhood,
    string City,
    BrazilianState State);