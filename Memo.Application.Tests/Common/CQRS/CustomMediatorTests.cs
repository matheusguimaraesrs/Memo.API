using Memo.Application.Common.CQRS;
using NSubstitute;

namespace Memo.Application.Tests.Common.CQRS;

public class CustomMediatorTests
{
    public record TestRequest(string Data) : IRequest<string>;
    
    private readonly IServiceProvider _serviceProvider = Substitute.For<IServiceProvider>();
    private readonly CustomMediator _mediator;

    public CustomMediatorTests()
    {
        _mediator = new CustomMediator(_serviceProvider);
    }

    [Fact]
    public async Task SendAsync_When_Handler_Exists()
    {
        var request = new TestRequest("Test");
        string expectedResponse = "Success";
        
        var handler = Substitute.For<IRequestHandler<TestRequest, string>>();
        handler.HandleAsync(request, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(expectedResponse));

        _serviceProvider.GetService(typeof(IRequestHandler<TestRequest, string>)).Returns(handler);

        // Act
        var result = await _mediator.SendAsync(request, CancellationToken.None);

        // Assert
        Assert.Equal(expectedResponse, result);
        await handler.Received(1).HandleAsync(request, Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task SendAsync_When_Handler_Throws_Propagates_Original_Exception()
    {
        var request = new TestRequest("Test");
        var handler = Substitute.For<IRequestHandler<TestRequest, string>>();
        handler.HandleAsync(request, Arg.Any<CancellationToken>())
            .Returns<Task<string>>(_ => throw new Memo.Domain.Exceptions.DomainException("boom"));

        _serviceProvider.GetService(typeof(IRequestHandler<TestRequest, string>)).Returns(handler);

        await Assert.ThrowsAsync<Memo.Domain.Exceptions.DomainException>(() =>
            _mediator.SendAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task SendAsync_When_Handler_Not_Registered()
    {
        // Arrange
        var request = new TestRequest("Test");
        _serviceProvider.GetService(typeof(IRequestHandler<TestRequest, string>)).Returns(null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _mediator.SendAsync(request, CancellationToken.None));

        Assert.Contains("No handler registered", exception.Message);
    }

}