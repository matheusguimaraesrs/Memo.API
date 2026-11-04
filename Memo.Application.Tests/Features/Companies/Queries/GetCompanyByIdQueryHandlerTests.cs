using FluentAssertions;
using Memo.Application.Features.Companies.Queries.GetById;
using Memo.Domain.Entities;
using Memo.Domain.IRepositories;
using NSubstitute;

namespace Memo.Application.Tests.Features.Companies.Queries;

public sealed class GetCompanyByIdQueryHandlerTests
{
    private readonly ICompanyRepository _repository = Substitute.For<ICompanyRepository>();
    private readonly GetCompanyByIdQueryHandler _handler;

    public GetCompanyByIdQueryHandlerTests()
    {
        _handler = new GetCompanyByIdQueryHandler(_repository);
    }

    [Fact]
    public async Task HandleAsync_WhenCompanyExist()
    {
        List<Company> companies = new List<Company>
        {
            new Company("Company One", 80),
            new Company("Company Two", 70)
        };

        _repository.GetByIdAsync(companies[0].Id).Returns(companies[0]);

        var query = new GetCompanyByIdQuery(companies[0].Id);
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(companies[0].Id);
        result.Name.Should().Be("Company One");
        result.Commission.Should().Be(80);

        await _repository.Received(1).GetByIdAsync(companies[0].Id); 
    }
}