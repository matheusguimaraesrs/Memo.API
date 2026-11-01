using FluentAssertions;
using FluentValidation;
using Memo.Application.Features.Companies.Commands.Create;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.Exceptions;
using Memo.Domain.IRepositories;
using NSubstitute;

namespace Memo.Application.Tests.Features.Companies.Commands;

public sealed class CreateCompanyCommandHandlerTests
{
    private readonly ICompanyRepository _repository = Substitute.For<ICompanyRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly CreateCompanyCommandHandler _handler;

    public CreateCompanyCommandHandlerTests()
    {
        _handler = new CreateCompanyCommandHandler(_repository, _unitOfWork);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand()
    {
        Company? company = null;
        _repository.Add(Arg.Do<Company>(c => company = c))
            .Returns(info => info.Arg<Company>());

        var command = new CreateCompanyCommand("Name Test", 80);
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        company.Should().NotBeNull();
        company.Id.Should().NotBeEmpty();
        company.Name.Should().Be("Name Test");
        company.Commission.Should().Be(80);
        result.Should().Be(company.Id);
        _repository.Received(1).Add(Arg.Any<Company>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_InvalidNameCommand()
    {
        var command = new CreateCompanyCommand("", 80);

        Func<Task> act = async () => await _handler.HandleAsync(command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
        _repository.DidNotReceive().Add(Arg.Any<Company>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task HandleAsync_InvalidCommissionCommand()
    {
        var command = new CreateCompanyCommand("Valid Name", 200);

        Func<Task> act = async () => await _handler.HandleAsync(command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
        _repository.DidNotReceive().Add(Arg.Any<Company>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}