using Api.Utils;
using Domain.ErrorHandling;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Api.Middlewares;

public class ErrorHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var errorMessage = $"Exception caught: {ex.Message}";

            _logger.LogError(errorMessage, ex);

            HttpResponseHandler.HandleError(context, StatusCodes.Status500InternalServerError, errorMessage);
            return;
                        
        }
    }
}
