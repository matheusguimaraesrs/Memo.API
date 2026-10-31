using FluentAssertions;
using Memo.Domain.Entities;
using Memo.Infra.Persistence;
using Memo.Infra.Repositories;
using Memo.Infra.Services;
using Memo.Infra.Tests.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Memo.Infra.Tests.Repositories;

public sealed class CompanyRepositoryTests
{
    private static Company CreateCompany(string name = "test company")
    {
        return new Company(name, 80);
    }

    private static CompanyRepository CreateRepository(AppDbContext context)
    {
        return new CompanyRepository(context, new PaginationService());
    }

    [Fact]
    public async Task Should_Create_Company()
    {
        await using var context = DbContextFactory.Create();
        var repository = CreateRepository(context);
        var company = CreateCompany();

        var result = repository.Add(company);
        await context.SaveChangesAsync();

        result.Should().BeSameAs(company);
        var savedCompany = await context.Companies.FindAsync(company.Id);
        savedCompany.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_Return_Company_By_Id()
    {
        await using var context = DbContextFactory.Create();
        var repository = CreateRepository(context);
        var company = CreateCompany();

        repository.Add(company);
        await context.SaveChangesAsync();

        var result = await repository.GetByIdAsync(company.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(company.Id);
    }

    [Fact]
    public async Task Should_Return_Null_When_Company_Does_Not_Exist()
    {
        await using var context = DbContextFactory.Create();
        var repository = CreateRepository(context);

        var result = await repository.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task Should_Return_Paged_Companies()
    {
        await using var context = DbContextFactory.Create();
        var repository = CreateRepository(context);
        repository.Add(CreateCompany("first"));
        repository.Add(CreateCompany("second"));
        repository.Add(CreateCompany("third"));
        await context.SaveChangesAsync();

        var result = await repository.GetPagedAsync(1, 20);
        
        result.Items.Should().HaveCount(3);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(20);
        result.TotalItems.Should().Be(3);
        result.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task Should_Update_Company()
    {
        await using var context = DbContextFactory.Create();
        var repository = CreateRepository(context);
        var company = CreateCompany();

        repository.Add(company);
        await context.SaveChangesAsync();
        
        company.Update("Updated name", 70);
        repository.Update(company);
        await context.SaveChangesAsync();

        var updatedCompany = await repository.GetByIdAsync(company.Id);
        updatedCompany!.Name.Should().Be(company.Name);
        updatedCompany.Commission.Should().Be(company.Commission);
    }
    
    [Fact]
    public async Task Should_Soft_Delete_Company()
    {
        await using var context = DbContextFactory.Create();
        var repository = CreateRepository(context);
        var company = CreateCompany();
        repository.Add(company);
        await context.SaveChangesAsync();

        repository.Delete(company);
        await context.SaveChangesAsync();

        var deletedCompany = await repository.GetByIdAsync(company.Id);
        deletedCompany.Should().BeNull();
        deletedCompany = await context.Companies
             .IgnoreQueryFilters()
             .SingleAsync(c => c.Id == company.Id);
         deletedCompany.IsDeleted.Should().BeTrue();
    }
}