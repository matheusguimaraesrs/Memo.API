using Memo.Domain.Common;
using Memo.Domain.Exceptions;

namespace Memo.Domain.Entities;

public sealed class Product : AuditableEntity
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public decimal UnitPrice { get; private set; }
    public Guid CompanyId { get; private set; }

    private Product() { }

    public Product(string name, string description, decimal unitPrice, Guid companyId)
    {
        Validate(name, description, unitPrice, companyId);
        Name = name;
        Description = description;
        UnitPrice = unitPrice;
        CompanyId = companyId;
    }

    public void Update(string name, string description, decimal unitPrice, Guid companyId)
    {
        Validate(name, description, unitPrice, companyId);
        Name = name;
        Description = description;
        UnitPrice = unitPrice;
        CompanyId = companyId;
    }

    private static void Validate(string name, string description, decimal unitPrice, Guid companyId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Description cannot be null or empty.");
        if (unitPrice <= 0)
            throw new DomainException("Price cannot be zero or negative.");
        if (companyId == Guid.Empty)
            throw new DomainException("Company cannot be null or empty.");
    }
}