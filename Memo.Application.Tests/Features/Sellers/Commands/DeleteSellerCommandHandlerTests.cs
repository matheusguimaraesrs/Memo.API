using FluentAssertions;
using Memo.Application.Features.Sellers.Commands.Delete;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.Exceptions;
using Memo.Domain.IRepositories;
using NSubstitute;

namespace Memo.Application.Tests.Features.Sellers.Commands;

public sealed class DeleteSellerCommandHandlerTests
{
    private readonly ISellerRepository _repository = Substitute.For<ISellerRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly DeleteSellerCommandHandler _handler;

    public DeleteSellerCommandHandlerTests()
    {
        _handler = new DeleteSellerCommandHandler(_repository, _unitOfWork);
    }

    [Fact]
    public async Task HandleAsync_WhenSellerExists()
    {
        Seller seller = new Seller("Test Name", "test.login", "StrongPassword");
        _repository.GetByIdAsync(seller.Id).Returns(seller);

        var command = new DeleteSellerCommand(seller.Id);

        var result = await _handler.HandleAsync(command, CancellationToken.None);
        
        result.Should().Be(seller.Id);
        
        await _repository.Received(1).DeleteAsync(seller);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task HandleAsync_WhenSellerDoesNotExist()
    {
        var nonExistentId = Guid.NewGuid();

        _repository.GetByIdAsync(nonExistentId)
            .Returns(Task.FromResult<Seller?>(null));

        var command = new DeleteSellerCommand(nonExistentId);

        Func<Task> act = async () => await _handler.HandleAsync(command, CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException<Seller>>();

        await _repository.DidNotReceive().DeleteAsync(Arg.Any<Seller>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}