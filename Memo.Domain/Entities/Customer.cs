using Memo.Domain.Common;
using Memo.Domain.Exceptions;

namespace Memo.Domain.Entities;

public sealed class Customer : AuditableEntity
{
    public string Name { get; private set; } = null!;
    public string Address { get; private set; } = null!;

    private Customer() { }

    public Customer(string name, string address)
    {
        Validate(name, address);
        Name = name;
        Address = address;
    }

    public void Update(string name, string address)
    {
        Validate(name, address);
        Name = name;
        Address = address;
    }

    private static void Validate(string name, string address)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Customer name cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(address))
            throw new DomainException("Address cannot be null or empty.");
    }
}
