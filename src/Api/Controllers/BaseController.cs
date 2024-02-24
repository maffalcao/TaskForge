using Api.Utils;
using Domain.ErrorHandling;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace Api.Controllers;

public class BaseController : ControllerBase
{
        
    public readonly ILogger<ProjectController> _logger;

    public BaseController(ILogger<ProjectController> logger)
    {
        _logger = logger;        
    }

    protected int GetAuthenticatedUserId() => int.Parse(HttpContext.Items["userId"]?.ToString());

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(context.ModelState);
        }
    }

    protected ActionResult<OperationResult> HandleResult(OperationResult result)
    {
        var statusCode = result?.Errors?.FirstOrDefault()?.StatusCode ?? StatusCodes.Status200OK;

        return new ObjectResult(result)
        {
            StatusCode = statusCode,
            ContentTypes = ["application/json"]
        };   
    }
}
