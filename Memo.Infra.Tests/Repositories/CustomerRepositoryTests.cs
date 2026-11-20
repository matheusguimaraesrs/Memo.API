using FluentAssertions;
using Memo.Domain.Entities;
using Memo.Domain.Enums;
using Memo.Domain.ValueObjects;
using Memo.Infra.Persistence;
using Memo.Infra.Repositories;
using Memo.Infra.Services;
using Memo.Infra.Tests.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Memo.Infra.Tests.Repositories;

public sealed class CustomerRepositoryTests
{
    private static Customer CreateCustomer(
        string name = "Test customer",
        Address? address = null)
    {
        return new Customer(name, address ?? new Address(
            "01001000", "100", "Main Street", "Downtown", "São Paulo", BrazilianState.SP));
    }

    private static CustomerRepository CreateRepository(AppDbContext context)
    {
        return new CustomerRepository(context, new PaginationService());
    }

    [Fact]
    public async Task Should_Create_Customer()
    {
        await using var context = DbContextFactory.Create();
        var repository = CreateRepository(context);
        var customer = CreateCustomer();

        var result = repository.Add(customer);
        await context.SaveChangesAsync(CancellationToken.None);

        result.Should().BeSameAs(customer);
        (await context.Customers.FindAsync(customer.Id)).Should().NotBeNull();
    }

    [Fact]
    public async Task Should_Return_Customer_By_Id()
    {
        await using var context = DbContextFactory.Create();
        var repository = CreateRepository(context);
        var customer = CreateCustomer();
        repository.Add(customer);
        await context.SaveChangesAsync(CancellationToken.None);

        var result = await repository.GetByIdAsync(customer.Id, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(customer.Id);
    }

    [Fact]
    public async Task Should_Return_Null_When_Customer_Does_Not_Exist()
    {
        await using var context = DbContextFactory.Create();
        var repository = CreateRepository(context);

        var result = await repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Should_Return_Paged_Customers()
    {
        await using var context = DbContextFactory.Create();
        var repository = CreateRepository(context);
        repository.Add(CreateCustomer("first"));
        repository.Add(CreateCustomer("second"));
        repository.Add(CreateCustomer("third"));
        await context.SaveChangesAsync(CancellationToken.None);

        var result = await repository.GetPagedAsync(1, 20, CancellationToken.None);

        result.Items.Should().HaveCount(3);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(20);
        result.TotalItems.Should().Be(3);
        result.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task Should_Update_Customer()
    {
        await using var context = DbContextFactory.Create();
        var repository = CreateRepository(context);
        var customer = CreateCustomer();
        repository.Add(customer);
        await context.SaveChangesAsync(CancellationToken.None);

        Address updatedAddress = new(
            "80010000", "200", "New Street", "Centro", "Curitiba", BrazilianState.PR);
        customer.Update("Updated name", updatedAddress);
        repository.Update(customer);
        await context.SaveChangesAsync(CancellationToken.None);

        var updatedCustomer = await repository.GetByIdAsync(customer.Id, CancellationToken.None);
        updatedCustomer!.Name.Should().Be("Updated name");
        updatedCustomer.Address.PostalCode.Should().Be("80010000");
        updatedCustomer.Address.City.Should().Be("Curitiba");
    }

    [Fact]
    public async Task Should_Soft_Delete_Customer()
    {
        await using var context = DbContextFactory.Create();
        var repository = CreateRepository(context);
        var customer = CreateCustomer();
        repository.Add(customer);
        await context.SaveChangesAsync(CancellationToken.None);

        repository.Delete(customer);
        await context.SaveChangesAsync(CancellationToken.None);

        (await repository.GetByIdAsync(customer.Id, CancellationToken.None)).Should().BeNull();
        var deletedCustomer = await context.Customers
            .IgnoreQueryFilters()
            .SingleAsync(value => value.Id == customer.Id);
        deletedCustomer.IsDeleted.Should().BeTrue();
    }
}
