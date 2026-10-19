using FluentValidation;
using Memo.Application.Common.CQRS;
using Microsoft.Extensions.DependencyInjection;

namespace Memo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        //FluentValidation.DependencyInjection
        services.AddValidatorsFromAssembly(typeof(DependencyInjection)
            .Assembly,
            includeInternalTypes: true);
        
        //CQRS
        services.AddTransient<IMediator, CustomMediator>();
        services.Scan(scan => scan
            .FromAssemblyOf<IMediator>()
            .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<,>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithTransientLifetime());
        return services;
    }
}