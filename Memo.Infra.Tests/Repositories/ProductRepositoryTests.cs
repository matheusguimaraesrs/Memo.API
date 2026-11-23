using FluentAssertions;
using Memo.Domain.Entities;
using Memo.Infra.Persistence;
using Memo.Infra.Repositories;
using Memo.Infra.Services;
using Memo.Infra.Tests.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Memo.Infra.Tests.Repositories;

public sealed class ProductRepositoryTests
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

    private static ProductRepository CreateRepository(AppDbContext context)
    {
        return new ProductRepository(context, new PaginationService());
    }

    [Fact]
    public async Task Should_Create_Product()
    {
        await using var context = DbContextFactory.Create();
        var repository = CreateRepository(context);
        var product = CreateProduct();

        var result = repository.Add(product);
        await context.SaveChangesAsync(CancellationToken.None);

        result.Should().BeSameAs(product);
        (await context.Products.FindAsync(product.Id)).Should().NotBeNull();
    }

    [Fact]
    public async Task Should_Return_Product_By_Id()
    {
        await using var context = DbContextFactory.Create();
        var repository = CreateRepository(context);
        var product = CreateProduct();

        repository.Add(product);
        await context.SaveChangesAsync(CancellationToken.None);

        var result = await repository.GetByIdAsync(
            product.Id,
            CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(product.Id);
    }

    [Fact]
    public async Task Should_Return_Null_When_Product_Does_Not_Exist()
    {
        await using var context = DbContextFactory.Create();
        var repository = CreateRepository(context);

        var result = await repository.GetByIdAsync(
            Guid.NewGuid(),
            CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Should_Return_Paged_Products()
    {
        await using var context = DbContextFactory.Create();
        var repository = CreateRepository(context);

        repository.Add(CreateProduct("first"));
        repository.Add(CreateProduct("second"));
        repository.Add(CreateProduct("third"));
        await context.SaveChangesAsync(CancellationToken.None);

        var result = await repository.GetPagedAsync(
            1,
            20,
            CancellationToken.None);

        result.Items.Should().HaveCount(3);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(20);
        result.TotalItems.Should().Be(3);
        result.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task Should_Update_Product()
    {
        await using var context = DbContextFactory.Create();
        var repository = CreateRepository(context);
        var product = CreateProduct();

        repository.Add(product);
        await context.SaveChangesAsync(CancellationToken.None);

        Guid updatedCompanyId = Guid.NewGuid();
        product.Update(
            "Updated product",
            "Updated description",
            25m,
            updatedCompanyId);
        repository.Update(product);
        await context.SaveChangesAsync(CancellationToken.None);

        var updatedProduct = await repository.GetByIdAsync(
            product.Id,
            CancellationToken.None);

        updatedProduct.Should().NotBeNull();
        updatedProduct!.Name.Should().Be("Updated product");
        updatedProduct.Description.Should().Be("Updated description");
        updatedProduct.UnitPrice.Should().Be(25m);
        updatedProduct.CompanyId.Should().Be(updatedCompanyId);
    }

    [Fact]
    public async Task Should_Soft_Delete_Product()
    {
        await using var context = DbContextFactory.Create();
        var repository = CreateRepository(context);
        var product = CreateProduct();

        repository.Add(product);
        await context.SaveChangesAsync(CancellationToken.None);

        repository.Delete(product);
        await context.SaveChangesAsync(CancellationToken.None);

        (await repository.GetByIdAsync(
            product.Id,
            CancellationToken.None)).Should().BeNull();

        var deletedProduct = await context.Products
            .IgnoreQueryFilters()
            .SingleAsync(value => value.Id == product.Id);

        deletedProduct.IsDeleted.Should().BeTrue();
        deletedProduct.DeletedAt.Should().NotBeNull();
        deletedProduct.DeletedBy.Should().Be("test-user");
    }
}
