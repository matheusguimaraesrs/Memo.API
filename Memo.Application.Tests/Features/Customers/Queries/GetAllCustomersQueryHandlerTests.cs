using FluentAssertions;
using Memo.Application.Features.Customers.Queries.GetAll;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.IRepositories;
using NSubstitute;

namespace Memo.Application.Tests.Features.Customers.Queries;

public sealed class GetAllCustomersQueryHandlerTests
{
    private readonly ICustomerRepository _repository = Substitute.For<ICustomerRepository>();
    private readonly GetAllCustomersQueryHandler _handler;

    public GetAllCustomersQueryHandlerTests()
    {
        _handler = new GetAllCustomersQueryHandler(_repository);
    }

    [Fact]
    public async Task HandleAsync_WhenCustomersExist()
    {
        List<Customer> customers =
        [
            new("Customer One", "Address One"),
            new("Customer Two", "Address Two")
        ];
        _repository.GetPagedAsync(1, 10, Arg.Any<CancellationToken>())
            .Returns(new PagedList<Customer>(customers, 1, 10, 12));

        var result = await _handler.HandleAsync(
            new GetAllCustomersQuery(1, 10),
            CancellationToken.None);

        result.Items.Should().HaveCount(2);
        result.Items.Select(value => value.Name)
            .Should().Contain(["Customer One", "Customer Two"]);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalItems.Should().Be(12);
        result.TotalPages.Should().Be(2);
        await _repository.Received(1).GetPagedAsync(1, 10, Arg.Any<CancellationToken>());
    }
}
