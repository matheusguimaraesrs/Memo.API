using FluentAssertions;
using Memo.Domain.Entities;
using Memo.Infra.Persistence;
using Memo.Infra.Repositories;
using Memo.Infra.Services;
using Memo.Infra.Tests.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Memo.Infra.Tests.Repositories;

public sealed class SellerRepositoryTests
{
    private static Seller CreateSeller(string name = "Test seller")
    {
        return new Seller(name, "test.login", "test-password");
    }

    private static SellerRepository CreateRepository(AppDbContext context)
    {
        return new SellerRepository(context, new PaginationService());
    }
    
    [Fact]
    public async Task Should_Create_Seller()
    {
        await using var context = DbContextFactory.Create();
        var repository = CreateRepository(context);
        var seller = CreateSeller();

        var result = repository.Add(seller);
        await context.SaveChangesAsync();
        
        result.Should().BeSameAs(seller);
        var savedSeller = await context.Sellers.FindAsync(seller.Id);
        savedSeller.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_Return_Seller_By_Id()
    {
        await using var context = DbContextFactory.Create();
        var repository = CreateRepository(context);
        var seller = CreateSeller();
        
        repository.Add(seller);
        await context.SaveChangesAsync();

        var result = await repository.GetByIdAsync(seller.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(seller.Id);
    }

    [Fact]
    public async Task Should_Return_Null_When_Seller_Does_Not_Exist()
    {
        await using var context = DbContextFactory.Create();
        var repository = CreateRepository(context);

        var result = await repository.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task Should_Return_Paged_Sellers()
    {
        await using var context = DbContextFactory.Create();
        var repository = CreateRepository(context);
        repository.Add(CreateSeller("first"));
        repository.Add(CreateSeller("second"));
        repository.Add(CreateSeller("third"));
        await context.SaveChangesAsync();

        var result = await repository.GetPagedAsync(1, 20);

        result.Items.Should().HaveCount(3);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(20);
        result.TotalItems.Should().Be(3);
        result.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task Should_Update_Seller()
    {
        await using var context = DbContextFactory.Create();
        var repository = CreateRepository(context);
        var seller = CreateSeller();
        
        repository.Add(seller);
        await context.SaveChangesAsync();

        seller.Update("Updated seller", "updated.login", "updated-password");
        repository.Update(seller);
        await context.SaveChangesAsync();

        var updatedSeller = await repository.GetByIdAsync(seller.Id);
        updatedSeller!.Name.Should().Be(seller.Name);
        updatedSeller.Login.Should().Be(seller.Login);
        updatedSeller.Password.Should().Be(seller.Password);
    }

    [Fact]
    public async Task Should_Soft_Delete_Seller()
    {
        await using var context = DbContextFactory.Create();
        var repository = CreateRepository(context);
        var seller = CreateSeller();
        repository.Add(seller);
        await context.SaveChangesAsync();

        repository.Delete(seller);
        await context.SaveChangesAsync();

        var deletedSeller = await repository.GetByIdAsync(seller.Id);
        deletedSeller.Should().BeNull();
        deletedSeller = await context.Sellers
            .IgnoreQueryFilters()
            .SingleAsync(c => c.Id == seller.Id);
        deletedSeller.IsDeleted.Should().BeTrue();
    }
}