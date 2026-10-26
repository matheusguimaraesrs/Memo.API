using FluentAssertions;
using Memo.Domain.Entities;
using Memo.Infra.Repositories;
using Memo.Infra.Tests.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Memo.Infra.Tests.Repositories;

public sealed class SellerRepositoryTests
{
    private static Seller CreateSeller(string login = "test.login")
    {
        return new Seller("Test seller", login, "test-password");
    }
    
    [Fact]
    public async Task Should_Create_Seller()
    {
        await using var context = DbContextFactory.Create();
        var repository = new SellerRepository(context);
        var seller = CreateSeller();

        var result = await repository.AddAsync(seller);

        result.Should().BeSameAs(seller);
        var savedSeller = await context.Sellers.FindAsync(seller.Id);
        savedSeller.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_Return_Seller_By_Id()
    {
        await using var context = DbContextFactory.Create();
        var repository = new SellerRepository(context);
        var seller = CreateSeller();
        await repository.AddAsync(seller);

        var result = await repository.GetByIdAsync(seller.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(seller.Id);
    }

    [Fact]
    public async Task Should_Return_Null_When_Seller_Does_Not_Exist()
    {
        await using var context = DbContextFactory.Create();
        var repository = new SellerRepository(context);

        var result = await repository.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task Should_Return_Paged_Sellers()
    {
        await using var context = DbContextFactory.Create();
        var repository = new SellerRepository(context);
        await repository.AddAsync(CreateSeller("first.login"));
        await repository.AddAsync(CreateSeller("second.login"));
        await repository.AddAsync(CreateSeller("third.login"));

        var result = await repository.GetPagedAsync(2, 2);

        result.Items.Should().HaveCount(1);
        result.Page.Should().Be(2);
        result.PageSize.Should().Be(2);
        result.TotalItems.Should().Be(3);
        result.TotalPages.Should().Be(2);
    }

    [Fact]
    public async Task Should_Update_Seller()
    {
        await using var context = DbContextFactory.Create();
        var repository = new SellerRepository(context);
        var seller = CreateSeller();
        await repository.AddAsync(seller);

        seller.Update("Updated seller", "updated.login", "updated-password");
        await repository.UpdateAsync(seller);

        var updatedSeller = await repository.GetByIdAsync(seller.Id);
        updatedSeller!.Name.Should().Be("Updated seller");
        updatedSeller.Login.Should().Be("updated.login");
    }

    [Fact]
    public async Task Should_Soft_Delete_Seller()
    {
        await using var context = DbContextFactory.Create();
        var repository = new SellerRepository(context);
        var seller = CreateSeller();
        await repository.AddAsync(seller);

        await repository.DeleteAsync(seller);

        (await repository.GetByIdAsync(seller.Id)).Should().BeNull();
        var deletedSeller = await context.Sellers
            .IgnoreQueryFilters()
            .SingleAsync(item => item.Id == seller.Id);
        deletedSeller.IsDeleted.Should().BeTrue();
    }

}