namespace Memo.Application.Common.CQRS;

public sealed class CustomMediator(IServiceProvider serviceProvider) : IMediator
{
    public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        
        var handler = serviceProvider.GetService(handlerType) ??
                      throw new InvalidOperationException($"No handler registered for '{requestType.Name}'.");
        
        var method = handlerType.GetMethod(nameof(IRequestHandler<IRequest<TResponse>, TResponse>.HandleAsync));
        return (Task<TResponse>)method!.Invoke(handler, [request, ct])!;
    }
}