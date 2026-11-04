using FluentAssertions;
using Memo.Application.Features.Companies.Commands.Update;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.Exceptions;
using Memo.Domain.IRepositories;
using NSubstitute;

namespace Memo.Application.Tests.Features.Companies.Commands;

public sealed class UpdateCompanyCommandHandlerTests
{
    private readonly ICompanyRepository _repository = Substitute.For<ICompanyRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly UpdateCompanyCommandHandler _handler;

    public UpdateCompanyCommandHandlerTests()
    {
        _handler = new UpdateCompanyCommandHandler(_repository, _unitOfWork);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand()
    {
        Company? company = new Company("Old Name", 80);
        _repository.GetByIdAsync(company.Id).Returns(company);

        var command = new UpdateCompanyCommand(company.Id, "New Name", 70);
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        company.Name.Should().Be("New Name");
        company.Commission.Should().Be(70);
        result.Should().Be(company.Id);

        _repository.Received(1).Update(company);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_InvalidCommand()
    {
        var command = new UpdateCompanyCommand(Guid.Empty, "", 0);

        Func<Task> act = async () => await _handler.HandleAsync(command, CancellationToken.None);
        await act.Should().ThrowAsync<EntityNotFoundException<Company>>();

        _repository.DidNotReceive().Update(Arg.Any<Company>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}