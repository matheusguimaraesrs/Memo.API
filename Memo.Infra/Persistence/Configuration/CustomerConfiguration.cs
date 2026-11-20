using Memo.Domain.Entities;
using Memo.Domain.Enums;
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
        builder.OwnsOne(customer => customer.Address, address =>
        {
            address.Property(value => value.PostalCode)
                .HasColumnName("PostalCode")
                .HasMaxLength(8)
                .IsRequired();
            address.Property(value => value.Number)
                .HasColumnName("AddressNumber")
                .HasMaxLength(20)
                .IsRequired();
            address.Property(value => value.Street)
                .HasColumnName("Street")
                .HasMaxLength(150)
                .IsRequired();
            address.Property(value => value.Neighborhood)
                .HasColumnName("Neighborhood")
                .HasMaxLength(100)
                .IsRequired();
            address.Property(value => value.City)
                .HasColumnName("City")
                .HasMaxLength(100)
                .IsRequired();
            address.Property(value => value.State)
                .HasConversion<string>()
                .HasColumnName("State")
                .HasMaxLength(2)
                .IsRequired();
        });
    }
}