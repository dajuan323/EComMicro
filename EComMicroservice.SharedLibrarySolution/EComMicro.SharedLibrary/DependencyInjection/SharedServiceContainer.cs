using EComMicro.SharedLibrary.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace EComMicro.SharedLibrary.DependencyInjection;

public static class SharedServiceContainer
{
    public static IServiceCollection AddSharedServices<TContext>
        (this IServiceCollection services, IConfiguration config, string fileName) where TContext : DbContext
    {
        // Add Generic DB context
        services.AddDbContext<TContext>(option => option.UseSqlServer(
            config
            .GetConnectionString("eCommerceConnection"), sqlserverOption =>
            sqlserverOption.EnableRetryOnFailure()));

        // Configure serilog logging
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Debug()
            .WriteTo.Console()
            .WriteTo.File(path: $"{fileName}~.text",
            restrictedToMinimumLevel: LogEventLevel.Information,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {message:lj} {NewLine}{Exception}",
            rollingInterval: RollingInterval.Day)
            .CreateLogger();

        // Add Jwt Auth Schee
        JWTAuthenticationScheme.AddJWTAuthenticationScheme(services, config);
        return services;
    }

    public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
    {
        // use global exception
        app.UseMiddleware<GlobalException>();

        // Register middleware to block all outside API calls
        //app.UseMiddleware<ListenOnlyToApiGateway>();

        return app;
    }
}
