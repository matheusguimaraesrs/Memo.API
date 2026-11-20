using FluentAssertions;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.Exceptions;
using Xunit;

namespace Memo.Domain.Tests.Entities;

public sealed class CustomerTests
{
    private static Customer CreateCustomer(
        string name = "Test customer",
        string address = "Test address")
    {
        return new Customer(name, address);
    }

    [Fact]
    public void Create_WithValidParameters()
    {
        Customer customer = CreateCustomer();

        customer.Id.Should().NotBeEmpty();
        customer.Name.Should().Be("Test customer");
        customer.Address.Should().Be("Test address");
        customer.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        customer.UpdatedAt.Should().BeNull();
        customer.UpdatedBy.Should().BeNull();
        customer.IsDeleted.Should().BeFalse();
    }

    [Theory]
    [InlineData("", "Valid address")]
    [InlineData("   ", "Valid address")]
    [InlineData(null, "Valid address")]
    [InlineData("Valid name", "")]
    [InlineData("Valid name", "   ")]
    [InlineData("Valid name", null)]
    public void Create_WithInvalidParameters(string? name, string? address)
    {
        Action action = () => CreateCustomer(name!, address!);

        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void Update_WithValidParameters()
    {
        Customer customer = CreateCustomer();

        customer.Update("New name", "New address");

        customer.Name.Should().Be("New name");
        customer.Address.Should().Be("New address");
    }

    [Theory]
    [InlineData("", "Valid address")]
    [InlineData("   ", "Valid address")]
    [InlineData(null, "Valid address")]
    [InlineData("Valid name", "")]
    [InlineData("Valid name", "   ")]
    [InlineData("Valid name", null)]
    public void Update_WithInvalidParameters(string? name, string? address)
    {
        Customer customer = CreateCustomer();

        Action action = () => customer.Update(name!, address!);

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
