using FluentAssertions;
using Memo.Application.Features.Companies.Commands.Delete;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.Exceptions;
using Memo.Domain.IRepositories;
using NSubstitute;

namespace Memo.Application.Tests.Features.Companies.Commands;

public sealed class DeleteCompanyCommandHandlerTests
{
    private readonly ICompanyRepository _repository = Substitute.For<ICompanyRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly DeleteCompanyCommandHandler _handler;

    public DeleteCompanyCommandHandlerTests()
    {
        _handler = new DeleteCompanyCommandHandler(_repository, _unitOfWork);
    }
    
    [Fact]
    public async Task HandleAsync_WhenCompanyExists()
    {
        Company company = new Company("test name", 80);
        _repository.GetByIdAsync(company.Id).Returns(company);

        var command = new DeleteCompanyCommand(company.Id);
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.Should().Be(company.Id);

        _repository.Received(1).Delete(company);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenCompanyDoesNotExist()
    {
        var nonExistentId = Guid.NewGuid();

        _repository.GetByIdAsync(nonExistentId).Returns(Task.FromResult<Company?>(null));

        var command = new DeleteCompanyCommand(nonExistentId);
        Func<Task> act = async () => await _handler.HandleAsync(command, CancellationToken.None);
        await act.Should().ThrowAsync<EntityNotFoundException<Company>>();

        _repository.DidNotReceive().Delete(Arg.Any<Company>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}