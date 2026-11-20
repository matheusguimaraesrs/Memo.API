using Memo.Domain.Enums;
using Memo.Domain.Exceptions;

namespace Memo.Domain.ValueObjects;

public sealed record Address
{
    public string PostalCode { get; private set; } = null!;
    public string Number { get; private set; } = null!;
    public string Street { get; private set; } = null!;
    public string Neighborhood { get; private set; } = null!;
    public string City { get; private set; } = null!;
    public BrazilianState State { get; private set; }

    private Address() { }

    public Address(
        string postalCode,
        string number, 
        string street, 
        string neighborhood, 
        string city, 
        BrazilianState state)
    {
        Validate(postalCode, number, street, neighborhood, city, state);
        
        PostalCode = postalCode;
        Number = number;
        Street = street;
        Neighborhood = neighborhood;
        City = city;
        State = state;
    }

    private static void Validate(
        string postalCode,
        string number,
        string street,
        string neighborhood,
        string city,
        BrazilianState state)
    {
        if (string.IsNullOrWhiteSpace(postalCode))
            throw new DomainException("Postal code cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(number))
            throw new DomainException("Address number cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(street))
            throw new DomainException("Street cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(neighborhood))
            throw new DomainException("Neighborhood cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(city))
            throw new DomainException("City cannot be null or empty.");
        if (!Enum.IsDefined(state))
            throw new DomainException("State is invalid.");
    }
}