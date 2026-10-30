using Memo.Domain.Common;
using Memo.Domain.IRepositories;
using Memo.Infra.Persistence;
using Memo.Infra.Repositories;
using Memo.Infra.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Memo.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPaginationService, PaginationService>();

        services.AddScoped<ISellerRepository, SellerRepository>();
        services.AddScoped<IRepresentedCompanyRepository, RepresentedCompanyRepository>();
        
        return services;
    }
}