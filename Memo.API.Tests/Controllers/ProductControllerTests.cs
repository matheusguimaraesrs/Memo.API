using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Memo.API.Controllers;
using Memo.Application.Common.CQRS;
using Memo.Application.Features.Products.Commands.Create;
using Memo.Application.Features.Products.Commands.Delete;
using Memo.Application.Features.Products.Commands.Update;
using Memo.Application.Features.Products.Queries.GetAll;
using Memo.Application.Features.Products.Queries.GetById;
using Memo.Application.Features.Products.Responses;
using Memo.Domain.Common;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace Memo.API.Tests.Controllers;

public sealed class ProductControllerTests
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly ProductController _controller;

    public ProductControllerTests()
    {
        _controller = new ProductController(_mediator);
    }

    [Fact]
    public async Task GetAllPaged_Should_Return_Ok()
    {
        var response = new PagedList<ProductResponse>([], 1, 10, 0);
        _mediator.SendAsync(Arg.Any<GetAllProductsQuery>(), Arg.Any<CancellationToken>())
            .Returns(response);

        var result = await _controller.GetAllPaged();

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeSameAs(response);
        await _mediator.Received(1).SendAsync(
            new GetAllProductsQuery(1, 10),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetById_Should_Return_Ok()
    {
        var id = Guid.NewGuid();
        var response = new ProductResponse(id, "Test Product", "Description", 10m, Guid.NewGuid());
        _mediator.SendAsync(Arg.Any<GetProductByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(response);

        var result = await _controller.GetById(id);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeSameAs(response);
        await _mediator.Received(1).SendAsync(
            new GetProductByIdQuery(id),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_Should_Return_CreatedAtAction_With_Created_Id()
    {
        var id = Guid.NewGuid();
        var command = new CreateProductCommand("Test Product", null, 10m, Guid.NewGuid());
        _mediator.SendAsync(command, Arg.Any<CancellationToken>()).Returns(id);

        var result = await _controller.Create(command);

        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(ProductController.GetById));
        createdResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(id);
        createdResult.Value.Should().Be(id);
        await _mediator.Received(1).SendAsync(command, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_Should_Use_Route_Id_And_Return_NoContent()
    {
        var id = Guid.NewGuid();
        var command = new UpdateProductCommand(
            Guid.NewGuid(),
            "Updated Product",
            null,
            25m,
            Guid.NewGuid());

        var result = await _controller.Update(id, command);

        result.Should().BeOfType<NoContentResult>();
        await _mediator.Received(1).SendAsync(
            command with { Id = id },
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Delete_Should_Forward_Id()
    {
        var id = Guid.NewGuid();

        var result = await _controller.Delete(id);

        result.Should().BeOfType<NoContentResult>();
        await _mediator.Received(1).SendAsync(
            new DeleteProductCommand(id),
            Arg.Any<CancellationToken>());
    }
}
