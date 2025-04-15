using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using EComMicro.SharedLibrary.DependencyInjection;
using ApiGateway.Presentation.Middleware;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot().AddCacheManager(x => x.WithDictionaryHandle());
JWTAuthenticationScheme.AddJWTAuthenticationScheme(builder.Services, builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});

var app = builder.Build();

app.UseCors();
app.UseMiddleware<AttachSignatureToRequest>();
app.UseOcelot().Wait();
app.UseHttpsRedirection();

app.Run();


