using Domain.ErrorHandling;
using Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class BaseController : ControllerBase
{
        
    public readonly ILogger<ProjectController> _logger;

    public BaseController(ILogger<ProjectController> logger)
    {
        _logger = logger;        
    }

    protected int GetAuthenticatedUserId() => int.Parse(HttpContext.Items["userId"].ToString());

    protected ActionResult<OperationResult> HandleResult<T>(OperationResult result) where T : class
    {
        if (result.IsSuccess)
        {
            return Ok(result.Result as T);
        }

        _logger.LogError(result.Error.Description);

        return StatusCode(result.Error.StatusCode, result);
        
    }


}
