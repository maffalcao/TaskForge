using System.Text;

public class RequestResponseLoggingMiddleware : IMiddleware
{
    private readonly ILogger _logger;

    public RequestResponseLoggingMiddleware(ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        _logger.LogInformation($"Request: {context.Request.Method} {context.Request.Path}");

        // Enable request body buffering
        context.Request.EnableBuffering();

        if (context.Request.ContentLength > 0)
        {
            string requestBody;
            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
            {
                requestBody = await reader.ReadToEndAsync();
                _logger.LogInformation($"Request Body: {requestBody}");

                // Reset the stream position if needed
                context.Request.Body.Seek(0, SeekOrigin.Begin);
            }
        }

        var originalBodyStream = context.Response.Body;
        using (var responseBody = new MemoryStream())
        {
            context.Response.Body = responseBody;

            await next(context);

            responseBody.Seek(0, SeekOrigin.Begin);
            var response = await new StreamReader(responseBody).ReadToEndAsync();
            _logger.LogInformation($"Response: {response}");

            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }

}
