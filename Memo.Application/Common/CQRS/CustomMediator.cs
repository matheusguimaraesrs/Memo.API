using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Memo.Application.Common.CQRS;

public sealed class CustomMediator(IServiceProvider serviceProvider) : IMediator
{
    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();
        
        var validatorType = typeof(IValidator<>).MakeGenericType(requestType);
        var validator = serviceProvider.GetService(validatorType) as IValidator;

        if (validator != null)
        {
            var contextType = typeof(ValidationContext<>).MakeGenericType(requestType);
            var context = (IValidationContext)Activator.CreateInstance(contextType, request)!;
            var validationResult = await validator.ValidateAsync(context, ct);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);
        }
        
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        var handler = serviceProvider.GetRequiredService(handlerType);

        var method = handlerType.GetMethod("HandleAsync");
        if (method == null)
            throw new InvalidOperationException($"HandleAsync method not found on {handlerType.Name}");

        return await (Task<TResponse>)method.Invoke(handler, new object[] { request, ct })!;
    }
}