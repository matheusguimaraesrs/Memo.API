using FluentAssertions;
using Memo.Application.Features.Customers;
using Memo.Application.Features.Customers.Commands.Update;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.Enums;
using Memo.Domain.Exceptions;
using Memo.Domain.IRepositories;
using Memo.Domain.ValueObjects;
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
        Address oldAddress = new(
            "01001000", "100", "Old Street", "Old Neighborhood", "São Paulo", BrazilianState.SP);
        Customer customer = new("Old name", oldAddress);
        _repository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);

        var address = new AddressInput(
            "80010000", "200", "New Street", "Centro", "Curitiba", BrazilianState.PR);
        var command = new UpdateCustomerCommand(customer.Id, "New name", address);
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        customer.Name.Should().Be("New name");
        customer.Address.City.Should().Be("Curitiba");
        customer.Address.State.Should().Be(BrazilianState.PR);
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
            new UpdateCustomerCommand(
                id,
                "Name",
                new AddressInput("01001000", "100", "Street", "Neighborhood", "City", BrazilianState.SP)),
            CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException<Customer>>();
        _repository.DidNotReceive().Update(Arg.Any<Customer>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
