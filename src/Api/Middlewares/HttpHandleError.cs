namespace Api.Middlewares;

public static class HttpHandleError 
{
    public static void HandleError(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var errorResponse = new
        {
            StatusCode = statusCode,
            Message = message
        };

        context.Response.WriteAsJsonAsync(errorResponse);
    }
}
