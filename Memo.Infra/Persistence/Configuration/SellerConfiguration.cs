using Memo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Memo.Infra.Persistence.Configuration;

internal sealed class SellerConfiguration : IEntityTypeConfiguration<Seller>
{
    public void Configure(EntityTypeBuilder<Seller> builder)
    {
        builder.ToTable("Sellers");
        builder.HasKey(seller => seller.Id);
        builder.Property(seller => seller.Name).IsRequired().HasMaxLength(100);
        builder.Property(seller => seller.Login).IsRequired().HasMaxLength(15);
        builder.HasIndex(seller => seller.Login).IsUnique();
        builder.Property(seller => seller.Password).IsRequired().HasMaxLength(20);
    }
}