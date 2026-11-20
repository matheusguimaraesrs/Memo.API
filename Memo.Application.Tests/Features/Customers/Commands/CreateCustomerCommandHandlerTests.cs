using FluentAssertions;
using Memo.Application.Features.Customers;
using Memo.Application.Features.Customers.Commands.Create;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.Enums;
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

        var address = new AddressInput(
            "01001000", "100", "Main Street", "Downtown", "São Paulo", BrazilianState.SP);
        var command = new CreateCustomerCommand("Test customer", address);
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        customer.Should().NotBeNull();
        customer!.Name.Should().Be("Test customer");
        customer.Address.City.Should().Be("São Paulo");
        customer.Address.State.Should().Be(BrazilianState.SP);
        result.Should().Be(customer.Id);
        _repository.Received(1).Add(Arg.Any<Customer>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("", "Valid address")]
    [InlineData("Valid name", "")]
    public async Task HandleAsync_InvalidCommand(string name, string address)
    {
        var addressInput = new AddressInput(
            "01001000", "100", address, "Downtown", "São Paulo", BrazilianState.SP);
        var command = new CreateCustomerCommand(name, addressInput);

        Func<Task> act = async () => await _handler.HandleAsync(command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
        _repository.DidNotReceive().Add(Arg.Any<Customer>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
