using System.Net;
using System.Security.Authentication;
using Fin.Domain.Exceptions;

namespace Fin.Api;

public class ErrorHandlingMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = exception switch
        {
            ArgumentNullException => HttpStatusCode.BadRequest,
            UserNotFoundException => HttpStatusCode.Unauthorized,
            AuthenticationException => HttpStatusCode.Unauthorized,
            UserAlreadExistsException => HttpStatusCode.Forbidden,
            _ => HttpStatusCode.InternalServerError
        };
        
        var result = System.Text.Json.JsonSerializer.Serialize(new { error = exception.Message }); 
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        return context.Response.WriteAsync(result);
    }
}