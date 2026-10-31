using Memo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Memo.Infra.Persistence.Configuration;

internal sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies");
        builder.HasKey(company => company.Id);
        builder.Property(company => company.Name).IsRequired().HasMaxLength(100);
        builder.HasIndex(company => company.Name).IsUnique();
        builder.Property(company => company.Commission).IsRequired().HasPrecision(5, 2);
    }
}