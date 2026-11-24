using Memo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Memo.Infra.Persistence.Configuration;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(product => product.Id);
        builder.Property(product => product.Name).IsRequired().HasMaxLength(100);
        builder.HasIndex(product => product.Name).IsUnique().HasFilter(""" "IsDeleted" = false""");
        builder.Property(product => product.Description).HasMaxLength(255);
        builder.Property(product => product.UnitPrice).IsRequired().HasPrecision(12, 2);
        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(product => product.CompanyId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}