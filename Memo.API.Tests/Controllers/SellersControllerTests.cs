using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Memo.API.Controllers;
using Memo.Application.Common.CQRS;
using Memo.Application.Features.Sellers.Commands.Create;
using Memo.Application.Features.Sellers.Commands.Delete;
using Memo.Application.Features.Sellers.Commands.Update;
using Memo.Application.Features.Sellers.Queries.GetAll;
using Memo.Application.Features.Sellers.Queries.GetById;
using Memo.Application.Features.Sellers.Queries.Response;
using Memo.Domain.Common;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace Memo.API.Tests.Controllers;

public sealed class SellersControllerTests
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly SellersController _controller;
    private static PagedList<SellerResponse> CreatePagedResponse() =>
        new([], 0, 1, 10);

    public SellersControllerTests()
    {
        _controller = new SellersController(_mediator);
    }

    [Fact]
    public async Task GetAllPaged_Should_Return_Ok()
    {
        var response = CreatePagedResponse();
        _mediator.SendAsync(Arg.Any<GetAllSellersQuery>(), Arg.Any<CancellationToken>())
            .Returns(response);

        var result = await _controller.GetAllPaged();

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeSameAs(response);
        await _mediator.Received(1).SendAsync(
            new GetAllSellersQuery(1, 10),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAllPaged_Should_Forward_Pagination()
    {
        var response = CreatePagedResponse();
        _mediator.SendAsync(Arg.Any<GetAllSellersQuery>(), Arg.Any<CancellationToken>())
            .Returns(response);

        var result = await _controller.GetAllPaged(2, 25);

        result.Should().BeOfType<OkObjectResult>();
        await _mediator.Received(1).SendAsync(
            new GetAllSellersQuery(2, 25),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetById_Should_Return_Ok()
    {
        var id = Guid.NewGuid();
        var response = new SellerResponse(id, "John Test", "john.test");
        _mediator.SendAsync(Arg.Any<GetSellerByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(response);

        var result = await _controller.GetById(id);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeSameAs(response);
        await _mediator.Received(1).SendAsync(
            new GetSellerByIdQuery(id),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_Should_Return_CreatedAtAction_With_Created_Id()
    {
        var id = Guid.NewGuid();
        var command = new CreateSellerCommand("John Test", "john.test", "StrongPassword");
        _mediator.SendAsync(command, Arg.Any<CancellationToken>()).Returns(id);

        var result = await _controller.Create(command);

        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(SellersController.GetById));
        createdResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(id);
        createdResult.Value.Should().Be(id);
        await _mediator.Received(1).SendAsync(command, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_Should_Use_Route_Id_And_Return_NoContent()
    {
        var id = Guid.NewGuid();
        var command = new UpdateSellerCommand(
            Guid.NewGuid(),
            "Updated Name",
            "updated.login",
            "UpdatedPassword");

        var result = await _controller.Update(id, command);

        result.Should().BeOfType<NoContentResult>();
        await _mediator.Received(1).SendAsync(
            command with { Id = id },
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Delete_Should_Forward_Id_And_Cancellation_Token()
    {
        var id = Guid.NewGuid();
        using var cancellationSource = new CancellationTokenSource();
        var cancellationToken = cancellationSource.Token;

        var result = await _controller.Delete(id, cancellationToken);

        result.Should().BeOfType<NoContentResult>();
        await _mediator.Received(1).SendAsync(
            new DeleteSellerCommand(id),
            cancellationToken);
    }

}
