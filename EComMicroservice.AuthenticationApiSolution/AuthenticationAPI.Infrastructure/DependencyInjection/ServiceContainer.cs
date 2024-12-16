using AuthenticationAPI.App.Interfaces;
using AuthenticationAPI.Infrastructure.Data;
using AuthenticationAPI.Infrastructure.Repositories;
using EComMicro.SharedLibrary.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthenticationAPI.Infrastructure.DependencyInjection;

public static class ServiceContainer
{
    public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
    {
        // Add Database connectivity
        // JWT Add Authentication Scheme
        SharedServiceContainer.AddSharedServices<AuthenticationDbContext>(services, config, config["MySerilog:FileName"]!);

        // Create Dependency Injection
        services.AddScoped<IUser, UserRepository>();

        return services;
    }
}
