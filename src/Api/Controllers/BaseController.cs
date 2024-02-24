using Api.Utils;
using Domain.ErrorHandling;
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
        
        if (result.IsSuccess)
        {
            HttpResponseHandler.HandleSuccess(HttpContext, result);
            return Ok();
        }

        else
        {
            var serializedResult = JsonConvert.SerializeObject(result);
            _logger.LogError(JsonConvert.SerializeObject(result));           

            HttpResponseHandler.HandleError(HttpContext, result);

            return BadRequest();
        }
        
        
    }


}
