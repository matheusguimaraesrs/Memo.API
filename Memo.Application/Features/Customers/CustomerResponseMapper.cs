using Memo.Application.Features.Customers.Response;
using Memo.Domain.Entities;
using Memo.Domain.ValueObjects;

namespace Memo.Application.Features.Customers;

internal static class CustomerResponseMapper
{
    public static AddressResponse ToResponse(Address address)
    {
        return new AddressResponse(
            address.PostalCode,
            address.Number,
            address.Street,
            address.Neighborhood,
            address.City,
            address.State);
    }

    public static CustomerResponse ToResponse(Customer customer)
    {
        return new CustomerResponse(
            customer.Id,
            customer.Name,
            ToResponse(customer.Address));
    }
}