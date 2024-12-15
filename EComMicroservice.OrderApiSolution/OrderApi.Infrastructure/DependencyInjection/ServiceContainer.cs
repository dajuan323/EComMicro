using EComMicro.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.App.Interfaces;
using OrderApi.Infrastructure.Data;
using OrderApi.Infrastructure.Repositories;

namespace OrderApi.Infrastructure.DependencyInjection;

public static class ServiceContainer
{
    public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
    {
        // Add Database Connectivity
        // Add Authentication scheme
        SharedServiceContainer.AddSharedServices<OrderDbContext>(services, config, config["MySerilog:FileName"]!);

        // Create Dependency Injection
        services.AddScoped<IOrder, OrderRepository>();

        return services;
    }

    public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
    {
        // Register middleware such as:
        // Global Exception => handle external errors
        // ListenToApiGateway Only => block all outside calls.
        SharedServiceContainer.UseSharedPolicies(app);

        return app;
    }
}
