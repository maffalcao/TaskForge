using Domain.Dtos;
using Domain.Entities;
using Domain.ErrorHandling;
using Domain.Interfaces.Persistence;
using Domain.Interfaces.Services;
using Mapster;

namespace Domain.Services;

public class ProjectService(
    IProjectRepository projectRepository, 
    IRepository<User> userRepository,
    IRepository<ProjectTask> taskRepository) : IProjectService
{
    public async Task<OperationResult> AddAsync(AddProjectDto projectDto, int userId)
    {
        if ((await UserExist(userId)) is false)
        {
            return OperationResult.Failure(OperationErrors.UserNotFound(userId));            
        }

        var project = new Project(projectDto.Name, userId);
        project = await projectRepository.AddProjectAsync(project);

        return OperationResult.Success(project.Adapt<ProjectDto>());

    }
    public async Task<bool> DeleteAsync(int id)
    {
        return await projectRepository.DeleteAsync(id);
    }

    public async Task<OperationResult> GetAllByUserIdAsync(int userId)
    {
        var projects = await projectRepository.GetAllByUserId(userId);

        return OperationResult.Success(projects.Adapt<IEnumerable<ProjectDto>>());
    }

    public async Task<ProjectDto> GetByIdAsync(int id)
    {
        var project = await projectRepository.GetByIdAsync(id);
        return project.Adapt<ProjectDto>();
    }

    public async Task<OperationResult> AddProjectTaskAsync(AddTaskDto addProjectTaskDto, 
        int projectId, int userId)
    {
        var user  = await userRepository.GetByIdAsync(projectId);
        if (user is null)
        {
            return OperationResult.Failure(OperationErrors.UserNotFound(userId));
        }

        if(addProjectTaskDto.AssignedUserId is not null)
        {
            var assignUserId = (int)addProjectTaskDto.AssignedUserId;
            if ((await UserExist(assignUserId) is false))
            {
                return OperationResult.Failure(OperationErrors.UserNotFound(assignUserId));
            }
        }

        var project = await projectRepository.GetByIdAsync(projectId);

        if (project is null)
        {
            return OperationResult.Failure(OperationErrors.ProjectNotFound(projectId));
        }

        var task = addProjectTaskDto.Adapt<ProjectTask>();
        task.ProjectId = projectId;        

        if(project.AddTask(task) is false)
        {
            return OperationResult.Failure(OperationErrors.ProjectMaxNumberOfTasksAchieved(projectId));
        }

        await projectRepository.UpdateAsync(project);

        return OperationResult.Success();

    }

    public async Task<OperationResult> GetTasksAsync(int projectId, int userId)
    {
        if ((await projectRepository.Exist(projectId)) is false)
        {
            return OperationResult.Failure(OperationErrors.ProjectNotFound(projectId));
        }

        if((await userRepository.Exist(userId)) is false)
        {
            return OperationResult.Failure(OperationErrors.UserNotFound(userId));
        }


        var tasks = await taskRepository.GetAsync(
            t => t.ProjectId == projectId, includeString: "AuditTrails");

        var tasksDto = GetTaskByProjectDtos(tasks);

        return OperationResult.Success(tasksDto);

    }

    private List<GetTaskByProjectDto> GetTaskByProjectDtos(IReadOnlyList<ProjectTask> tasks)
    {
        var tasksDtos = tasks.Adapt<IEnumerable<GetTaskByProjectDto>>().ToList();

        for (int i = 0; i < tasksDtos.Count(); i++)
        {
            if (tasks[i].AuditTrails is not null)
            {
                var taskComments = tasks[i]?.AuditTrails.Where(a => a.ChangedField == "Comment");

                if (taskComments.Any())
                {
                    tasksDtos[i].Comments = new List<TaskCommentDto>(
                        taskComments.Select(t =>
                            new TaskCommentDto(t.ModifiedByUserId, t.NewValue, t.CreatedAt)));

                }
            }
        }

        return tasksDtos;
    }


    private async Task<bool> UserExist(int userId) =>
        await userRepository.Exist(userId);

    
}