using Domain.Dtos;
using Domain.ErrorHandling;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Api.Utils;

public static class HttpResponseHandler
{
    public static void HandleError(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";        

        var error = new Error(
            "UnhandledException",
            message, statusCode);

        var result = OperationResult.Failure([error]);

        context.Response.WriteAsJsonAsync(result);
    }

    public static void HandleError(HttpContext context, OperationResult result)
    {
        var statusCode = result?.Errors?.FirstOrDefault()?.StatusCode ?? StatusCodes.Status404NotFound;
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        context.Response.WriteAsJsonAsync(result);
    }

    public static void HandleSuccess(HttpContext context, OperationResult result, int statusCode = (int)HttpStatusCode.OK)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";  
        context.Response.WriteAsJsonAsync(result);
    }
}
