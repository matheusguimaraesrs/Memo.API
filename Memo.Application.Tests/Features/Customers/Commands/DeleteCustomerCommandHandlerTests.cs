using FluentAssertions;
using Memo.Application.Features.Customers.Commands.Delete;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.Exceptions;
using Memo.Domain.IRepositories;
using NSubstitute;

namespace Memo.Application.Tests.Features.Customers.Commands;

public sealed class DeleteCustomerCommandHandlerTests
{
    private readonly ICustomerRepository _repository = Substitute.For<ICustomerRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly DeleteCustomerCommandHandler _handler;

    public DeleteCustomerCommandHandlerTests()
    {
        _handler = new DeleteCustomerCommandHandler(_repository, _unitOfWork);
    }

    [Fact]
    public async Task HandleAsync_WhenCustomerExists()
    {
        Customer customer = new("Test name", "Test address");
        _repository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);

        var result = await _handler.HandleAsync(
            new DeleteCustomerCommand(customer.Id),
            CancellationToken.None);

        result.Should().Be(customer.Id);
        _repository.Received(1).Delete(customer);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenCustomerDoesNotExist()
    {
        var id = Guid.NewGuid();
        _repository.GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Customer?>(null));

        Func<Task> act = async () => await _handler.HandleAsync(
            new DeleteCustomerCommand(id),
            CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException<Customer>>();
        _repository.DidNotReceive().Delete(Arg.Any<Customer>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
