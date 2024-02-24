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
        public readonly IProjectService _projectService;
        public readonly ITaskService _taskService;

        public ProjectController(ILogger<ProjectController> logger, 
            IProjectService projectService,
            ITaskService taskService) : base(logger) {

            _projectService = projectService;
            _taskService = taskService;
        }

        [HttpPost(Name = "AddProject")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(OperationResult), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<OperationResult>> AddProject([FromBody] AddProjectDto projectDto,
            [FromServices] IValidator<AddProjectDto> validator)
        {            
            var result = await _projectService.AddAsync(projectDto, GetAuthenticatedUserId());

            return HandleResult(result);
        }

        [HttpGet("{userId:int}", Name = "GetProjectsByUserId")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ProjectDto>> GetProjectsByUserId(int userId)
        {
            var result = await _projectService.GetAllByUserIdAsync(userId);

            return Ok(result);

        }

        [HttpPost("{projectId:int}/task", Name = "AddTask")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(OperationResult), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<OperationResult>> AddTask(int projectId, [FromBody] AddTaskDto dto)
        {
            var result = await _taskService.AddAsync(dto, projectId, GetAuthenticatedUserId());

            return HandleResult(result);

        }

        [HttpPut("{projectId:int}/task/{taskId:int}", Name = "UpdateTask")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(OperationResult), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<OperationResult>> UpdateTask(int projectId, int taskId, [FromBody] UpdateTaskDto dto)
        {
            var result = await _taskService.UpdateAsync(dto, taskId, GetAuthenticatedUserId());

            return HandleResult(result);            
        }

        [HttpDelete("{projectId:int}/task/{taskId:int}", Name = "DeleteTask")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]                
        [ProducesResponseType(typeof(OperationResult), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<OperationResult>> DeleteTask(int taskId)
        {
            var result = await _taskService.DeleteAsync(taskId, GetAuthenticatedUserId());

            return HandleResult(result);
        }
    }


}
