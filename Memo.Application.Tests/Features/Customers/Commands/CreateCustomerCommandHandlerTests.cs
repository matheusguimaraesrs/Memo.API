using FluentAssertions;
using Memo.Application.Features.Customers.Commands.Create;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.Exceptions;
using Memo.Domain.IRepositories;
using NSubstitute;

namespace Memo.Application.Tests.Features.Customers.Commands;

public sealed class CreateCustomerCommandHandlerTests
{
    private readonly ICustomerRepository _repository = Substitute.For<ICustomerRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly CreateCustomerCommandHandler _handler;

    public CreateCustomerCommandHandlerTests()
    {
        _handler = new CreateCustomerCommandHandler(_repository, _unitOfWork);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand()
    {
        Customer? customer = null;
        _repository.Add(Arg.Do<Customer>(value => customer = value))
            .Returns(info => info.Arg<Customer>());

        var command = new CreateCustomerCommand("Test customer", "Test address");
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        customer.Should().NotBeNull();
        customer!.Name.Should().Be("Test customer");
        customer.Address.Should().Be("Test address");
        result.Should().Be(customer.Id);
        _repository.Received(1).Add(Arg.Any<Customer>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("", "Valid address")]
    [InlineData("Valid name", "")]
    public async Task HandleAsync_InvalidCommand(string name, string address)
    {
        var command = new CreateCustomerCommand(name, address);

        Func<Task> act = async () => await _handler.HandleAsync(command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
        _repository.DidNotReceive().Add(Arg.Any<Customer>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
