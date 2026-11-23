using FluentAssertions;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.Exceptions;
using Xunit;

namespace Memo.Domain.Tests.Entities;

public sealed class ProductTests
{
    private static Product CreateProduct(
        string name = "Test product",
        string description = "Test description",
        decimal unitPrice = 10m,
        Guid? companyId = null)
    {
        return new Product(
            name,
            description,
            unitPrice,
            companyId ?? Guid.NewGuid());
    }

    [Fact]
    public void Create_WithValidParameters()
    {
        Guid companyId = Guid.NewGuid();

        Product product = CreateProduct(companyId: companyId);

        product.Id.Should().NotBeEmpty();
        product.Name.Should().Be("Test product");
        product.Description.Should().Be("Test description");
        product.UnitPrice.Should().Be(10m);
        product.CompanyId.Should().Be(companyId);
        product.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        product.UpdatedAt.Should().BeNull();
        product.UpdatedBy.Should().BeNull();
        product.IsDeleted.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithInvalidName(string? name)
    {
        Action action = () => CreateProduct(name: name!);

        action.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithOptionalDescription(string? description)
    {
        Product product = CreateProduct(description: description!);

        product.Description.Should().Be(description);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidUnitPrice(decimal unitPrice)
    {
        Action action = () => CreateProduct(unitPrice: unitPrice);

        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_WithEmptyCompanyId()
    {
        Action action = () => CreateProduct(companyId: Guid.Empty);

        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void Update_WithValidParameters()
    {
        Product product = CreateProduct();
        Guid companyId = Guid.NewGuid();

        product.Update("Updated product", "Updated description", 25m, companyId);

        product.Name.Should().Be("Updated product");
        product.Description.Should().Be("Updated description");
        product.UnitPrice.Should().Be(25m);
        product.CompanyId.Should().Be(companyId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Update_WithInvalidName(string? name)
    {
        Product product = CreateProduct();

        Action action = () => product.Update(
            name!,
            "Updated description",
            25m,
            Guid.NewGuid());

        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void SoftDelete_WithValidParameters()
    {
        Product product = CreateProduct();
        var auditable = (IAuditable)product;

        auditable.SoftDelete("admin");

        product.IsDeleted.Should().BeTrue();
        product.DeletedBy.Should().Be("admin");
        product.DeletedAt.Should().NotBeNull();
        product.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }
}
