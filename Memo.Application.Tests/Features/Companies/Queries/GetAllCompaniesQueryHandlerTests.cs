using FluentAssertions;
using Memo.Application.Features.Companies.Queries.GetAll;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.IRepositories;
using NSubstitute;

namespace Memo.Application.Tests.Features.Companies.Queries;

public sealed class GetAllCompaniesQueryHandlerTests
{
    private readonly ICompanyRepository _repository = Substitute.For<ICompanyRepository>();
    private readonly GetAllCompaniesQueryHandler _handler;

    public GetAllCompaniesQueryHandlerTests()
    {
        _handler = new GetAllCompaniesQueryHandler(_repository);
    }

    [Fact]
    public async Task HandleAsync_WhenCompaniesExist()
    {
        List<Company> companies = new List<Company>
        {
            new Company("Company One", 80),
            new Company("Company Two", 70),
        };

        _repository.GetPagedAsync(1, 10, Arg.Any<CancellationToken>())
            .Returns(new PagedList<Company>(companies, 1, 10, 12));

        var query = new GetAllCompaniesQuery();
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.Items.Select(x => x.Name).Should().Contain(new[] { "Company One", "Company Two" });
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalItems.Should().Be(12);
        result.TotalPages.Should().Be(2);

        await _repository.Received(1).GetPagedAsync(1, 10, Arg.Any<CancellationToken>());
    }
}