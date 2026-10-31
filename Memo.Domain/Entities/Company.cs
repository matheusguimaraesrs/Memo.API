using Memo.Domain.Common;
using Memo.Domain.Exceptions;

namespace Memo.Domain.Entities;

public sealed class Company : AuditableEntity
{
    public string Name { get; private set; }
    public decimal Commission { get; private set; }

    private Company() { }

    public Company(string name, decimal commission)
    {
        Validations(name, commission);

        Name = name;
        Commission = commission;
    }

    public void Update(string name, decimal commission)
    {
        Validations(name, commission);

        Name = name;
        Commission = commission;
    }

    private void Validations(string name, decimal commission)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Company name cannot be null or empty.");
        if (commission < 0 || commission > 100)
            throw new DomainException("Commission must be between 0% and 100%.");
    }
}