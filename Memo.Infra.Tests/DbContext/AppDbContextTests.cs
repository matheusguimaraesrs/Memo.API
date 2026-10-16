using FluentAssertions;
using Memo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Memo.Infra.Tests.DbContext;

public class AppDbContextTests
{
    [Fact]
    public async Task Should_Set_CreatedBy_When_Added()
    {
        await using var context = DbContextFactory.Create();

        var seller = new Seller(
            name: "Test seller",
            login: "test.login",
            password: "test-password");

        await context.Sellers.AddAsync(seller);
        await context.SaveChangesAsync();

        seller.CreatedBy.Should().Be("test-user");
        seller.CreatedAt.Should().NotBe(default);
    }

    [Fact]
    public async Task Should_Set_UpdatedBy_When_Modified()
    {
        await using var context = DbContextFactory.Create();

        var seller = new Seller("Test seller", "test.login", "test-password");

        await context.Sellers.AddAsync(seller);
        await context.SaveChangesAsync();

        var createdAt = seller.CreatedAt;
        var createdBy = seller.CreatedBy;

        seller.Update("Updated seller", "updated.login", "updated-password");
        await context.SaveChangesAsync();

        seller.UpdatedBy.Should().Be("test-user");
        seller.UpdatedAt.Should().NotBeNull();
        seller.CreatedAt.Should().Be(createdAt);
        seller.CreatedBy.Should().Be(createdBy);
    }

    [Fact]
    public async Task Should_Soft_Delete_When_Deleted()
    {
        await using var context = DbContextFactory.Create();

        var seller = new Seller("Test seller", "test.login", "test-password");

        await context.Sellers.AddAsync(seller);
        await context.SaveChangesAsync();

        context.Sellers.Remove(seller);
        await context.SaveChangesAsync();

        seller.IsDeleted.Should().BeTrue();
        seller.DeletedBy.Should().Be("test-user");
        seller.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_Not_Return_Soft_Deleted_Entities_By_Default()
    {
        await using var context = DbContextFactory.Create();

        var seller = new Seller("Test seller", "test.login", "test-password");

        await context.Sellers.AddAsync(seller);
        await context.SaveChangesAsync();

        context.Sellers.Remove(seller);
        await context.SaveChangesAsync();

        var result = await context.Sellers
            .FirstOrDefaultAsync(item => item.Id == seller.Id);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Should_Return_Soft_Deleted_Entities_When_Query_Filters_Are_Ignored()
    {
        await using var context = DbContextFactory.Create();

        var seller = new Seller("Test seller", "test.login", "test-password");

        await context.Sellers.AddAsync(seller);
        await context.SaveChangesAsync();

        context.Sellers.Remove(seller);
        await context.SaveChangesAsync();

        var result = await context.Sellers
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(item => item.Id == seller.Id);

        result.Should().NotBeNull();
        result!.IsDeleted.Should().BeTrue();
    }
}