using Memo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Memo.Infra.Persistence.Configuration;

internal sealed class RepresentedCompanyConfiguration : IEntityTypeConfiguration<RepresentedCompany>
{
    public void Configure(EntityTypeBuilder<RepresentedCompany> builder)
    {
        builder.ToTable("RepresentedCompanies");
        builder.HasKey(company => company.Id);
        builder.Property(company => company.CompanyName).IsRequired().HasMaxLength(100);
        builder.HasIndex(company => company.CompanyName).IsUnique();
        builder.Property(company => company.Commission).IsRequired().HasPrecision(5, 2);
    }
}