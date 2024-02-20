namespace Api.Middlewares;

public class ErrorHandlingMiddleware: IMiddleware
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
            var error = $"Exception caught: {ex.Message}";
            _logger.LogError(error, ex);

            HttpHandleError.HandleError(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            
        }
    }
}
