using System.Collections.Concurrent;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Memo.Application.Common.CQRS;

public sealed class CustomMediator(IServiceProvider serviceProvider) : IMediator
{
    private static readonly ConcurrentDictionary<(Type Request, Type Response), RequestExecutor> Executors = new();

    public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var executor = Executors.GetOrAdd(
            (request.GetType(), typeof(TResponse)),
            static key => (RequestExecutor)Activator.CreateInstance(
                typeof(RequestExecutor<,>).MakeGenericType(key.Request, key.Response))!);

        return ((RequestExecutor<TResponse>)executor).ExecuteAsync(serviceProvider, request, ct);
    }

    private abstract class RequestExecutor;

    private abstract class RequestExecutor<TResponse> : RequestExecutor
    {
        public abstract Task<TResponse> ExecuteAsync(
            IServiceProvider serviceProvider, IRequest<TResponse> request, CancellationToken ct);
    }

    private sealed class RequestExecutor<TRequest, TResponse> : RequestExecutor<TResponse>
        where TRequest : IRequest<TResponse>
    {
        public override async Task<TResponse> ExecuteAsync(
            IServiceProvider serviceProvider, IRequest<TResponse> request, CancellationToken ct)
        {
            var typedRequest = (TRequest)request;

            var validator = serviceProvider.GetService<IValidator<TRequest>>();
            if (validator != null)
            {
                var validationResult = await validator.ValidateAsync(typedRequest, ct);
                if (!validationResult.IsValid)
                    throw new ValidationException(validationResult.Errors);
            }

            var handler = serviceProvider.GetService<IRequestHandler<TRequest, TResponse>>()
                ?? throw new InvalidOperationException(
                    $"No handler registered for {typeof(TRequest).Name}.");

            return await handler.HandleAsync(typedRequest, ct);
        }
    }
}
