using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Memo.API.Controllers;
using Memo.Application.Common.CQRS;
using Memo.Application.Features.Customers.Commands.Create;
using Memo.Application.Features.Customers.Commands.Delete;
using Memo.Application.Features.Customers.Commands.Update;
using Memo.Application.Features.Customers.Queries.GetAll;
using Memo.Application.Features.Customers.Queries.GetById;
using Memo.Application.Features.Customers.Response;
using Memo.Domain.Common;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace Memo.API.Tests.Controllers;

public sealed class CustomerControllerTests
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly CustomerController _controller;

    private static PagedList<CustomerResponse> CreatePagedResponse() =>
        new([], 1, 10, 0);

    public CustomerControllerTests()
    {
        _controller = new CustomerController(_mediator);
    }

    [Fact]
    public async Task GetAll_Should_Return_Ok_And_Default_Pagination()
    {
        var response = CreatePagedResponse();
        _mediator.SendAsync(
                Arg.Any<GetAllCustomersQuery>(),
                Arg.Any<CancellationToken>())
            .Returns(response);

        var result = await _controller.GetAll();

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeSameAs(response);
        await _mediator.Received(1).SendAsync(
            new GetAllCustomersQuery(1, 10),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAll_Should_Forward_Pagination()
    {
        _mediator.SendAsync(
                Arg.Any<GetAllCustomersQuery>(),
                Arg.Any<CancellationToken>())
            .Returns(CreatePagedResponse());

        var result = await _controller.GetAll(2, 25);

        result.Should().BeOfType<OkObjectResult>();
        await _mediator.Received(1).SendAsync(
            new GetAllCustomersQuery(2, 25),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetById_Should_Return_Ok()
    {
        var id = Guid.NewGuid();
        var response = new CustomerResponse(id, "Test customer", "Test address");
        _mediator.SendAsync(
                Arg.Any<GetCustomerByIdQuery>(),
                Arg.Any<CancellationToken>())
            .Returns(response);

        var result = await _controller.GetById(id);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeSameAs(response);
        await _mediator.Received(1).SendAsync(
            new GetCustomerByIdQuery(id),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_Should_Return_CreatedAtAction()
    {
        var id = Guid.NewGuid();
        var command = new CreateCustomerCommand("Test customer", "Test address");
        _mediator.SendAsync(command, Arg.Any<CancellationToken>()).Returns(id);

        var result = await _controller.Create(command);

        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(CustomerController.GetById));
        createdResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(id);
        createdResult.Value.Should().Be(id);
        await _mediator.Received(1).SendAsync(command, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_Should_Use_Route_Id_And_Return_NoContent()
    {
        var id = Guid.NewGuid();
        var command = new UpdateCustomerCommand(
            Guid.NewGuid(),
            "Updated customer",
            "Updated address");

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
            new DeleteCustomerCommand(id),
            cancellationToken);
    }
}
