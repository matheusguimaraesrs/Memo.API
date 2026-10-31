using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Memo.API.Exceptions;
using Memo.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace Memo.API.Tests.Exceptions;

public sealed class GlobalExceptionHandlerTests
{
    private readonly GlobalExceptionHandler _handler =
        new(NullLogger<GlobalExceptionHandler>.Instance);

    [Fact]
    public async Task TryHandleAsync_Should_Return_BadRequest_For_ValidationException()
    {
        var exception = new ValidationException(
            [new ValidationFailure("Name", "Name is required.")]);

        var response = await HandleAsync(exception);

        response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        response.Body.Should().Contain("\"title\":\"Request Failed\"");
        response.Body.Should().Contain("\"Name\":[\"Name is required.\"]");
    }

    [Fact]
    public async Task TryHandleAsync_Should_Return_UnprocessableEntity_For_DomainException()
    {
        var exception = new DomainException("Invalid seller.");

        var response = await HandleAsync(exception);

        response.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
        response.Body.Should().Contain("\"title\":\"Business Rule Violation\"");
        response.Body.Should().Contain("\"detail\":\"Invalid seller.\"");
    }

    [Fact]
    public async Task TryHandleAsync_Should_Return_NotFound_For_EntityNotFoundException()
    {
        var exception = new EntityNotFoundException<TestEntity>();

        var response = await HandleAsync(exception);

        response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        response.Body.Should().Contain("\"title\":\"Not Found\"");
        response.Body.Should().Contain("\"detail\":\"TestEntity not found.\"");
    }

    [Fact]
    public async Task TryHandleAsync_Should_Return_InternalServerError_For_UnexpectedException()
    {
        var response = await HandleAsync(new InvalidOperationException("Unexpected failure."));

        response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        response.Body.Should().Contain("\"title\":\"Internal Server Error\"");
        response.Body.Should().Contain(
            "\"detail\":\"An unexpected error occurred. Please try again later.\"");
    }

    private async Task<(int StatusCode, string Body)> HandleAsync(Exception exception)
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/sellers";
        context.Response.Body = new MemoryStream();
        await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        context.Response.Body.Position = 0;
        using var reader = new StreamReader(context.Response.Body);
        return (context.Response.StatusCode, await reader.ReadToEndAsync());
    }

    private sealed class TestEntity
    {
    }
}
