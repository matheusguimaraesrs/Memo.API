using FluentAssertions;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.Exceptions;
using Xunit;

namespace Memo.Domain.Tests.Entities;

public sealed class SellerTests
{
    private static Seller CreateSeller(string login = "test.login")
    {
        return new Seller("Test seller", login, "test-password");
    }
    
    [Fact]
    public void Create_WithValidParameters()
    {
        Seller seller = CreateSeller();

        seller.Should().NotBeNull();
        seller.Id.Should().NotBeEmpty();
        seller.Name.Should().Be("Test seller"); 
        seller.Login.Should().Be("test.login");
        seller.Password.Should().Be("test-password");
        
        //Audit tests
        seller.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        seller.CreatedBy.Should().BeNull();
        seller.UpdatedBy.Should().BeNull();
        seller.UpdatedAt.Should().BeNull();
        seller.IsDeleted.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData(null)]
    public void Create_WithInvalidParameters(string? invalidName)
    {
        Action action = () => new Seller(invalidName!, "login", "StrongPassword");
        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void Update_WithValidParameters()
    {
        Seller seller = CreateSeller();

        string name = "New Name";
        string login = "new.login";
        string password = "new.password";
        
        seller.Update(name, login, password);

        seller.Name.Should().Be(name);
        seller.Login.Should().Be(login);
        seller.Password.Should().Be(password);
    }

    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData(null)]
    public void Update_WithInvalidParameters(string? invalidName)
    {
        Seller seller = CreateSeller();
        
        string login = "joao.silva";
        string password = "StrongPassword";

        Action action = () => seller.Update(invalidName!, login, password);
        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void SoftDelete_WithValidParameters()
    {
        Seller seller = CreateSeller();
        
        string admin = "admin";
        var auditable = (IAuditable)seller;
        
        auditable.SoftDelete(admin);

        seller.IsDeleted.Should().BeTrue();
        seller.DeletedBy.Should().Be(admin);
        seller.DeletedAt.Should().NotBeNull();
        seller.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }
}
