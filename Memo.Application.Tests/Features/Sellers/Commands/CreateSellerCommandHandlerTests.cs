using FluentAssertions;
using Memo.Application.Features.Sellers.Commands.Create;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.Exceptions;
using Memo.Domain.IRepositories;
using NSubstitute;

namespace Memo.Application.Tests.Features.Sellers.Commands;

public sealed class CreateSellerCommandHandlerTests
{
    private readonly ISellerRepository _repository = Substitute.For<ISellerRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly CreateSellerCommandHandler _handler;

    public CreateSellerCommandHandlerTests()
    {
        _handler = new CreateSellerCommandHandler(_repository, _unitOfWork);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand()
    {
        Seller? seller = null;
        _repository.AddAsync(Arg.Do<Seller>(s => seller = s))
            .Returns(info => info.Arg<Seller>());
        
        var command = new CreateSellerCommand("John Test", "john.test", "StrongPassword");
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        seller.Should().NotBeNull();
        seller.Id.Should().NotBeEmpty();
        seller.Name.Should().Be("John Test");
        seller.Login.Should().Be("john.test");
        seller.Password.Should().Be("StrongPassword");
        result.Should().Be(seller.Id);
        await _repository.Received(1).AddAsync(Arg.Any<Seller>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task HandleAsync_InvalidCommand()
    {
        var command = new CreateSellerCommand("", "", "");

        Func<Task> act = async () => await _handler.HandleAsync(command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();

        await _repository.DidNotReceive().AddAsync(Arg.Any<Seller>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}