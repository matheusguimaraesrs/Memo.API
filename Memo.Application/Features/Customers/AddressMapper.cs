using Memo.Domain.ValueObjects;

namespace Memo.Application.Features.Customers;

internal sealed class AddressMapper
{
    public static Address ToDomain(AddressInput input)
    {
        return new Address(
            input.PostalCode,
            input.Number,
            input.Street,
            input.Neighborhood,
            input.City,
            input.State);
    }
}