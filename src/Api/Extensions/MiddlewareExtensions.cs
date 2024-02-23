using Api.Middlewares;

namespace Api.Extensions;

public static class MiddlewareExtensions
{
    public static void ConfigureMiddlewares(this IServiceCollection services)
    {
        services.AddScoped<ValidateUserMiddleware>();
        services.AddScoped<ErrorHandlingMiddleware>();
        services.AddScoped<RequestResponseLoggingMiddleware>();

    }

    public static void UseMiddlewares(this WebApplication app)
    {
        app.UseMiddleware<ValidateUserMiddleware>();
        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.UseMiddleware<RequestResponseLoggingMiddleware>();

    }
}
