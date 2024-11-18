using EComMicro.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace EComMicro.SharedLibrary.Middleware;

public class GlobalException(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Declare default variables
        string message = "Sorry, internal server error occurred.";
        int statusCode = (int)HttpStatusCode.InternalServerError;
        string title = "Error";

        try
        {
            await next(context);

            // check if Response is too many requests //429
            if(context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
            {
                title = "Warning";
                message = "Too many requests made";
                statusCode = StatusCodes.Status429TooManyRequests;
                await ModifyHeader(context, title, message, statusCode);
            }

            // if Response is UnAuthorized // 401 Status Coe
            if(context.Response.StatusCode is StatusCodes.Status401Unauthorized)
            {
                title = "Alert";
                message = "You are not authorized to access.";
                statusCode = StatusCodes.Status401Unauthorized;
                await ModifyHeader(context, title, message, statusCode);
            }

            // if Response is Forbidden // 403
            if(context.Response.StatusCode is StatusCodes.Status403Forbidden)
            {
                title = "Out of Access";
                message = "You are not allowed access.";
                statusCode = StatusCodes.Status403Forbidden;
                await ModifyHeader(context, title, message, statusCode);
            }
        }
        catch (Exception ex)
        {
            // Log Original Exceptions / File, Console, Debugger
            LogException.LogExceptions(ex);

            // check if Exception is timeout // 408 Request Timeout
            if(ex is TaskCanceledException || ex is TimeoutException)
            {
                title = "Timed out...";
                message = "Request timeout...";
                statusCode = StatusCodes.Status408RequestTimeout;

            }

            // If Exception caught
            // if no exceptions, default
            await ModifyHeader(context, title, message, statusCode);
        }
    }

    private async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
    {
        // display scary-free message to client
        context.Response.ContentType = "Application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
        {
            Detail = message,
            Status = statusCode,
            Title = title
        }), CancellationToken.None);
        return;
    }
}
