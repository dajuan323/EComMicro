using Microsoft.AspNetCore.Http;

namespace EComMicro.SharedLibrary.Middleware;

public class ListenOnlyToApiGateway(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Extract specific header from the request
        var signedHeader = context.Request.Headers["Api-Gateway"];

        // NULL means, the request is not coming from the API Gateway // 503 service unvailable to user
        if(signedHeader.FirstOrDefault() == null)
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsync("Sorry, service is unavailable.");
            return;
        } else
        {
            await next(context);
        }
    }
}

