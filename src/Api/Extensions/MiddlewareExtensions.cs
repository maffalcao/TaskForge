using Api.Middlewares;

namespace Api.Extensions;

public static class MiddlewareExtensions
{
    public static void ConfigureMiddlewares(this IServiceCollection services)
    {
        services.AddScoped<ErrorHandlingMiddleware>();
        services.AddScoped<ValidateUserMiddleware>();
        services.AddScoped<RequestResponseLoggingMiddleware>();

    }

    public static void UseMiddlewares(this WebApplication app)
    {
        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.UseMiddleware<ValidateUserMiddleware>();
        app.UseMiddleware<RequestResponseLoggingMiddleware>();

    }
}
