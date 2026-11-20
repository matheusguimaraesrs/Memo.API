using Memo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Memo.Infra.Persistence.Configuration;

internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        builder.HasKey(customer => customer.Id);
        builder.Property(customer => customer.Name).IsRequired().HasMaxLength(100);
        builder.HasIndex(customer => customer.Name).IsUnique().HasFilter(""" "IsDeleted" = false""");
        builder.Property(customer => customer.Address).IsRequired().HasMaxLength(255);
    }
}