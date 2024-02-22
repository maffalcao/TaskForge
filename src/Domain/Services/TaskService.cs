using Domain.Dtos;
using Domain.Entities;
using Domain.ErrorHandling;
using Domain.Interfaces.Persistence;
using Domain.Interfaces.Services;
using Mapster;

namespace Domain.Services;

public class TaskService(
    IProjectRepository projectRepository, 
    IRepository<User> userRepository,
    IRepository<ProjectTask> taskRepository) : ITaskService
{
    public async Task<OperationResult> AddAsync(AddTaskDto addTaskDto, int projectId, int userId)
    {
        var user = await userRepository.GetByIdAsync(projectId);
        if (user is null)
        {
            return OperationResult.Failure(Errors.UserNotFound(userId));
        }

        if (addTaskDto.AssignedUserId is not null)
        {
            var assignUserId = (int)addTaskDto.AssignedUserId;
            if ((await userRepository.Exist(assignUserId) is false))
            {
                return OperationResult.Failure(Errors.UserNotFound(assignUserId));
            }
        }

        var project = (await projectRepository
            .GetAsync(p => p.Id == projectId, includeString: "Tasks"))
            .FirstOrDefault();

        if (project is null)
        {
            return OperationResult.Failure(Errors.ProjectNotFound(projectId));
        }

        var task = addTaskDto.Adapt<ProjectTask>();
        task.ProjectId = projectId;

        if (project.AddTask(task) is false)
        {
            return OperationResult.Failure(Errors.ProjectMaxNumberOfTasksAchieved(projectId));
        }

        var addedTask = await taskRepository.AddAsync(task);

        return OperationResult.Success(addedTask.Adapt<TaskDto>());

    }

}