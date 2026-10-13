using FluentAssertions;
using Memo.Domain.Entities;
using Xunit;

namespace Memo.Domain.Tests;

public class SellerTests
{
    [Fact]
    public void Constructor_WithValidParameters()
    {
        var name = "João Silva";
        var login = "joao.silva";
        var passwordHash = new byte[] { 1, 2, 3 };
        var passwordSalt = new byte[] { 4, 5, 6 };
        
        var seller = new Seller(name, login, passwordHash, passwordSalt);

        seller.Should().NotBeNull();
        seller.Id.Should().NotBeEmpty();
        seller.Name.Should().Be(name); 
        seller.Login.Should().Be(login);
        seller.PasswordHash.Should().BeEquivalentTo(passwordHash);
        seller.PasswordSalt.Should().BeEquivalentTo(passwordSalt);
    }
}