using FluentAssertions;
using Memo.Domain.Common;
using Memo.Domain.IRepositories;
using Memo.Infra.Persistence;
using Memo.Infra.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Memo.Infra.Tests.DependencyInjection;

public sealed class DependencyInjectionTests
{
    [Fact]
    public void Should_Register_Infrastructure_Dependencies()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        services.AddInfrastructure(configuration);

        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<IHttpContextAccessor>().Should().NotBeNull();
        provider.GetRequiredService<IUserContext>().Should().BeOfType<UserContext>();
        provider.GetRequiredService<ISellerRepository>().Should().NotBeNull();
        provider.GetRequiredService<AppDbContext>().Should().NotBeNull();
    }

    [Fact]
    public void Should_Register_UserContext_And_Repository_As_Scoped()
    {
        var services = new ServiceCollection();

        services.AddInfrastructure(CreateConfiguration());

        services.Should().Contain(descriptor =>
            descriptor.ServiceType == typeof(IUserContext) &&
            descriptor.Lifetime == ServiceLifetime.Scoped);

        services.Should().Contain(descriptor =>
            descriptor.ServiceType == typeof(ISellerRepository) &&
            descriptor.Lifetime == ServiceLifetime.Scoped);
    }

    private static IConfiguration CreateConfiguration()
    {
        var configuration = new ConfigurationManager();
        configuration["ConnectionStrings:DefaultConnection"] =
            "Host=localhost;Database=memo_test;Username=test;Password=test";

        return configuration;
    }
}
