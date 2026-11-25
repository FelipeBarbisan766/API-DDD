using System.Text.Json;
using API_DDD.Domain.Exceptions;

namespace API_DDD.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = ResolveStatusCode(exception);

        if (statusCode >= 500)
            _logger.LogError(exception, "Unhandled exception");
        else
            _logger.LogWarning("Handled exception: {Message}", exception.Message);

        var response = new
        {
            errorCode = ResolveErrorCode(exception),
            message = statusCode >= 500
                ? "An unexpected error occurred."
                : exception.Message,
            errors = (exception as ValidationException)?.Errors
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static int ResolveStatusCode(Exception exception) => exception switch
    {
        NotFoundException => StatusCodes.Status404NotFound,
        ForbiddenException => StatusCodes.Status403Forbidden,
        ConflictException => StatusCodes.Status409Conflict,
        ValidationException => StatusCodes.Status400BadRequest,
        _ => StatusCodes.Status500InternalServerError
    };

    private static string ResolveErrorCode(Exception exception) => exception switch
    {
        AppException appException => appException.ErrorCode,
        DomainException domainException => domainException.ErrorCode,
        _ => "INTERNAL_ERROR"
    };
}
