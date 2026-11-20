using FluentAssertions;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.Enums;
using Memo.Domain.Exceptions;
using Memo.Domain.ValueObjects;
using Xunit;

namespace Memo.Domain.Tests.Entities;

public sealed class CustomerTests
{
    private static Customer CreateCustomer(
        string name = "Test customer",
        Address? address = null)
    {
        return new Customer(name, address ?? CreateAddress());
    }

    private static Address CreateAddress()
    {
        return new Address(
            "01001000",
            "100",
            "Main Street",
            "Downtown",
            "São Paulo",
            BrazilianState.SP);
    }

    [Fact]
    public void Create_WithValidParameters()
    {
        Customer customer = CreateCustomer();

        customer.Id.Should().NotBeEmpty();
        customer.Name.Should().Be("Test customer");
        customer.Address.PostalCode.Should().Be("01001000");
        customer.Address.City.Should().Be("São Paulo");
        customer.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        customer.UpdatedAt.Should().BeNull();
        customer.UpdatedBy.Should().BeNull();
        customer.IsDeleted.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithInvalidParameters(string? name)
    {
        Action action = () => CreateCustomer(name!);

        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void Update_WithValidParameters()
    {
        Customer customer = CreateCustomer();
        Address address = new(
            "80010000",
            "200",
            "Other Street",
            "Centro",
            "Curitiba",
            BrazilianState.PR);

        customer.Update("New name", address);

        customer.Name.Should().Be("New name");
        customer.Address.Should().Be(address);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Update_WithInvalidParameters(string? name)
    {
        Customer customer = CreateCustomer();

        Action action = () => customer.Update(name!, CreateAddress());

        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void SoftDelete_WithValidParameters()
    {
        Customer customer = CreateCustomer();
        var auditable = (IAuditable)customer;

        auditable.SoftDelete("admin");

        customer.IsDeleted.Should().BeTrue();
        customer.DeletedBy.Should().Be("admin");
        customer.DeletedAt.Should().NotBeNull();
        customer.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }
}
