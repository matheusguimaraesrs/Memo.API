using Memo.Application.Common.CQRS;
using Microsoft.Extensions.DependencyInjection;

namespace Memo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IMediator, CustomMediator>();
        
        //CQRS
        services.Scan(scan => scan
            .FromAssemblyOf<IMediator>()
            .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<,>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithTransientLifetime());
        return services;
    }
}