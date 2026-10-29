using FluentAssertions;
using Memo.Application.Features.Sellers.Commands.Update;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.Exceptions;
using Memo.Domain.IRepositories;
using NSubstitute;

namespace Memo.Application.Tests.Features.Sellers.Commands;

public sealed class UpdateSellerCommandHandlerTests
{
    private readonly ISellerRepository _repository = Substitute.For<ISellerRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly UpdateSellerCommandHandler _handler;

    public UpdateSellerCommandHandlerTests()
    {
        _handler = new UpdateSellerCommandHandler(_repository, _unitOfWork);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand()
    {
        Seller? seller = new Seller("Old Name", "old.login", "StrongPassword");
        _repository.GetByIdAsync(seller.Id).Returns(seller);

        var command = new UpdateSellerCommand(seller.Id, "New Name", "new.login", "StrongPassword2");
        var result = await _handler.HandleAsync(command , CancellationToken.None);

        seller.Name.Should().Be("New Name");
        seller.Login.Should().Be("new.login");
        seller.Password.Should().Be("StrongPassword2");
        result.Should().Be(seller.Id);

        _repository.Received(1).Update(seller);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task HandleAsync_InvalidCommand()
    {
        var command = new UpdateSellerCommand(Guid.NewGuid(), "", "", "");

        Func<Task> act = async () => await _handler.HandleAsync(command, CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException<Seller>>();

        _repository.DidNotReceive().Update(Arg.Any<Seller>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
