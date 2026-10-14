using FluentAssertions;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.Exceptions;
using Xunit;

namespace Memo.Domain.Tests;

public class SellerTests
{
    [Fact]
    public void Create_WithValidParameters()
    {
        string name = "João Silva";
        string login = "joao.silva";
        string password = "StrongPassword";
        
        Seller seller = new Seller(name, login, password);

        seller.Should().NotBeNull();
        seller.Id.Should().NotBeEmpty();
        seller.Name.Should().Be(name); 
        seller.Login.Should().Be(login);
        seller.Password.Should().Be(password);
        
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
        string name = "João Silva";
        string login = "joao.silva";
        string password = "StrongPassword";
        
        Seller seller = new Seller(name, login, password);

        name = "New Name";
        login = "new.login";
        seller.Update(name, login, password);

        seller.Name.Should().Be(name);
        seller.Login.Should().Be(login);
    }

    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData(null)]
    public void Update_WithInvalidParameters(string? invalidName)
    {
        string name = "João Silva";
        string login = "joao.silva";
        string password = "StrongPassword";
        
        Seller seller = new Seller(name, login, password);
        Action action = () => seller.Update(invalidName!, login, password);
        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void SoftDelete_WithValidParameters()
    {
        string name = "João Silva";
        string login = "joao.silva";
        string password = "StrongPassword";

        Seller seller = new Seller(name, login, password);
        
        string admin = "admin";
        var auditable = (IAuditable)seller;
        
        auditable.SoftDelete(admin);

        seller.IsDeleted.Should().BeTrue();
        seller.DeletedBy.Should().Be(admin);
        seller.DeletedAt.Should().NotBeNull();
        seller.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }
}
