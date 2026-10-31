using FluentAssertions;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.Exceptions;
using Xunit;

namespace Memo.Domain.Tests.Entities;

public sealed class CompanyTests
{
    private static Company CreateCompany(string name = "test name")
    {
        return new Company(name, 80);
    }
    
    [Fact]
    public void Create_WithValidParameters()
    {
        Company company = CreateCompany();

        company.Should().NotBeNull();
        company.Id.Should().NotBeEmpty();
        company.CompanyName.Should().Be("test name");
        company.Commission.Should().Be(80);
        
        //Audit tests
        company.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        company.UpdatedBy.Should().BeNull();
        company.UpdatedAt.Should().BeNull();
        company.IsDeleted.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithInvalidParameters(string? invalidName)
    {
        Action action = () => CreateCompany(invalidName!);
        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void Update_WithValidParameters()
    {
        Company company = CreateCompany();
        company.Update("New Name", 70);

        company.CompanyName.Should().Be("New Name");
        company.Commission.Should().Be(70);
    }

    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData(null)]
    public void Upadate_WithInvalidParameters(string? invalidName)
    {
        Company company = CreateCompany();

        Action action = () => company.Update(invalidName!, 70);
        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void SoftDelete_WithValidParameters()
    {
        Company company = CreateCompany();

        string admin = "admin";
        var auditable = (IAuditable)company;
        
        auditable.SoftDelete(admin);

        company.IsDeleted.Should().BeTrue();
        company.DeletedBy.Should().Be(admin);
        company.DeletedAt.Should().NotBeNull();
        company.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }
}