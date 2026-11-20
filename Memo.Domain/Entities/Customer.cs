using Memo.Domain.Common;
using Memo.Domain.Exceptions;
using Memo.Domain.ValueObjects;

namespace Memo.Domain.Entities;

public sealed class Customer : AuditableEntity
{
    public string Name { get; private set; } = null!;
    public Address Address { get; private set; } = null!;

    private Customer() { }

    public Customer(string name, Address address)
    {
        Validate(name, address);
        Name = name;
        Address = address;
    }

    public void Update(string name, Address address)
    {
        Validate(name, address);
        Name = name;
        Address = address;
    }

    private static void Validate(string name, Address address)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Customer name cannot be null or empty.");
        if (address is null)
            throw new DomainException("Address cannot be null or empty.");
    }
}
