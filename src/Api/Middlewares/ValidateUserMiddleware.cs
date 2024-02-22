using Api.Middlewares;

public class ValidateUserMiddleware : IMiddleware
{
    ILogger<ValidateUserMiddleware> _logger;

    public ValidateUserMiddleware(ILogger<ValidateUserMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.Request.Headers.ContainsKey("userId"))
        {
            HttpHandleError.HandleError(context, StatusCodes.Status400BadRequest, "UserId header is missing!");
            return;
        }

        var userIdHeader = context.Request.Headers["userId"].ToString();

        if (int.TryParse(userIdHeader, out int userId))
        {
            context.Items["userId"] = userId;
        }
        else
        {
            HttpHandleError.HandleError(context, StatusCodes.Status400BadRequest, "Invalid userId' header value");
            return;
        }

        await next(context);
    }
}
