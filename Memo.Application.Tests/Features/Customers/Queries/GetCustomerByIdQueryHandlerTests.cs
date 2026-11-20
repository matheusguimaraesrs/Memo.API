using FluentAssertions;
using Memo.Application.Features.Customers.Queries.GetById;
using Memo.Domain.Entities;
using Memo.Domain.Exceptions;
using Memo.Domain.IRepositories;
using NSubstitute;

namespace Memo.Application.Tests.Features.Customers.Queries;

public sealed class GetCustomerByIdQueryHandlerTests
{
    private readonly ICustomerRepository _repository = Substitute.For<ICustomerRepository>();
    private readonly GetCustomerByIdQueryHandler _handler;

    public GetCustomerByIdQueryHandlerTests()
    {
        _handler = new GetCustomerByIdQueryHandler(_repository);
    }

    [Fact]
    public async Task HandleAsync_WhenCustomerExists()
    {
        Customer customer = new("Test customer", "Test address");
        _repository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);

        var result = await _handler.HandleAsync(
            new GetCustomerByIdQuery(customer.Id),
            CancellationToken.None);

        result.Id.Should().Be(customer.Id);
        result.Name.Should().Be("Test customer");
        result.Address.Should().Be("Test address");
        await _repository.Received(1).GetByIdAsync(
            customer.Id,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenCustomerDoesNotExist()
    {
        var id = Guid.NewGuid();
        _repository.GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Customer?>(null));

        Func<Task> act = async () => await _handler.HandleAsync(
            new GetCustomerByIdQuery(id),
            CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException<Customer>>();
    }
}
