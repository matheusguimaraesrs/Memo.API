using Memo.Domain.Common;
using Memo.Domain.Exceptions;

namespace Memo.Domain.Entities;

public sealed class RepresentedCompany : AuditableEntity
{
    public string CompanyName { get; private set; }
    public decimal Commission { get; private set; }

    private RepresentedCompany() { }

    public RepresentedCompany(string companyName, decimal commission)
    {
        Validations(companyName, commission);

        CompanyName = companyName;
        Commission = commission;
    }

    public void Update(string companyName, decimal commission)
    {
        Validations(companyName, commission);

        CompanyName = companyName;
        Commission = commission;
    }

    private void Validations(string companyName, decimal commission)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            throw new DomainException("Company name cannot be null or empty.");
        if (commission == 0)
            throw new DomainException("Commission cannot be zero.");
    }
}