using FluentAssertions;
using Memo.Application.Features.Customers.Commands.Update;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.Exceptions;
using Memo.Domain.IRepositories;
using NSubstitute;

namespace Memo.Application.Tests.Features.Customers.Commands;

public sealed class UpdateCustomerCommandHandlerTests
{
    private readonly ICustomerRepository _repository = Substitute.For<ICustomerRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly UpdateCustomerCommandHandler _handler;

    public UpdateCustomerCommandHandlerTests()
    {
        _handler = new UpdateCustomerCommandHandler(_repository, _unitOfWork);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand()
    {
        Customer customer = new("Old name", "Old address");
        _repository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);

        var command = new UpdateCustomerCommand(customer.Id, "New name", "New address");
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        customer.Name.Should().Be("New name");
        customer.Address.Should().Be("New address");
        result.Should().Be(customer.Id);
        _repository.Received(1).Update(customer);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenCustomerDoesNotExist()
    {
        var id = Guid.NewGuid();
        _repository.GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Customer?>(null));

        Func<Task> act = async () => await _handler.HandleAsync(
            new UpdateCustomerCommand(id, "Name", "Address"),
            CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException<Customer>>();
        _repository.DidNotReceive().Update(Arg.Any<Customer>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
