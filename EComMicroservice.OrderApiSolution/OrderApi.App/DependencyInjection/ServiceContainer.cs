using EComMicro.SharedLibrary.Logs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.App.Services;
using Polly;
using Polly.Retry;

namespace OrderApi.App.DependencyInjection;

public static class ServiceContainer
{
    public static IServiceCollection AddAppService(this IServiceCollection services, IConfiguration config)
    {
        // Register HttpClient service
        // Create Dependency Injection
        services.AddHttpClient<IOrderService, OrderService>(options =>
        {
            options.BaseAddress = new Uri(config["ApiGateway:BaseAddress"]!);
            options.Timeout = TimeSpan.FromSeconds(1);
        });

        // Create Retry Strategy
        var retryStrategy = new RetryStrategyOptions()
        {
            ShouldHandle = new PredicateBuilder().Handle<TaskCanceledException>(),
            BackoffType = DelayBackoffType.Constant,
            UseJitter = true,
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromMilliseconds(500),
            OnRetry = args =>
            {
                string message = $"OnRetry, Attempt: {args.AttemptNumber}, Outcome: {args.Outcome}";
                LogException.LogToConsole(message);
                LogException.LogToDebugger(message);
                return ValueTask.CompletedTask;
            }
        };

        // User Retry strategy
        services.AddResiliencePipeline("my-retry-pipeline", builder =>
        {
            builder.AddRetry(retryStrategy);
        });

        return services;
    }
}
