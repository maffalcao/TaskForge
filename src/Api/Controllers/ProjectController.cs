using Domain.Dtos;
using Domain.ErrorHandling;
using Domain.Interfaces.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProjectController : BaseController
    {

        public ProjectController(ILogger<ProjectController> logger, IProjectService projectService) : base(logger, projectService) { }

        [HttpPost(Name = "AddNewProject")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<OperationResult>> AddNewProject([FromBody] AddProjectDto projectDto,
            [FromServices] IValidator<AddProjectDto> validator)
        {
            var userId = GetAuthenticatedUserId();
            var result = await _projectService.AddAsync(projectDto, userId);

            return HandleResult<ProjectDto>(result);
        }

        [HttpGet("{userId:int}", Name = "GetProjectsByUserId")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<ProjectDto>> GetProjectsByUserId(int userId)
        {
            var result = await _projectService.GetAllByUserIdAsync(userId);

            return Ok(result);


        }
    }


}
