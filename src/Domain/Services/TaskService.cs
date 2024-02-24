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
        var project = (await projectRepository
            .GetAsync(p => p.Id == projectId, includeString: "Tasks"))
            .FirstOrDefault();


        if (project is null)
        {
            return OperationResult.Failure(OperationErrors.ProjectNotFound(projectId));
        }
        if ((await IsValidUser(userId)) is false)
        {
            return OperationResult.Failure(OperationErrors.UserNotFound(userId));
        }

        if (addTaskDto.AssignedUserId != null && (await IsValidUser(userId)) is false)
        {
            return OperationResult.Failure(OperationErrors.UserNotFound(userId));
        }


        var task = addTaskDto.Adapt<ProjectTask>();
        task.ProjectId = projectId;

        if (project.AddTask(task) is false)
        {
            return OperationResult.Failure(OperationErrors.ProjectMaxNumberOfTasksAchieved(projectId));
        }

        var addedTask = await taskRepository.AddAsync(task);

        return OperationResult.Success(addedTask.Adapt<TaskDto>());

    }

    public async Task<OperationResult> DeleteAsync(int taskId, int userId)
    {
        var task = await taskRepository.GetByIdAsync(taskId);

        if(!IsValidTask(task))
        {
            return OperationResult.Failure(OperationErrors.TaskNotFound(taskId));
        }

        if ((await IsValidUser(userId)) is false)
        {
            return OperationResult.Failure(OperationErrors.UserNotFound(userId));
        }

        task.DeletedAt = DateTime.UtcNow;
        task.ModifiedByUserId = userId;

        var deletedTask = await taskRepository.UpdateAsync(task);
        return OperationResult.Success(deletedTask.Adapt<TaskDto>());


    }

    public async Task<OperationResult> UpdateAsync(UpdateTaskDto updateTaskDto, int taskId, int userId)
    {
        var task = await taskRepository.GetByIdAsync(taskId);

        if (!IsValidTask(task))
        {
            return OperationResult.Failure(OperationErrors.TaskNotFound(taskId));
        }

        if ((await IsValidUser(userId)) is false)
        {
            return OperationResult.Failure(OperationErrors.UserNotFound(userId));
        }

        if (updateTaskDto.AssignedUserId != null && (await IsValidUser(userId)) is false)
        {
            return OperationResult.Failure(OperationErrors.UserNotFound(userId));
        }

        if (task.Priority != updateTaskDto.Priority)
        {
            return OperationResult.Failure(OperationErrors.TaskPriorityCantBechanged(taskId));
        }

        task.AssignedUserId = updateTaskDto.AssignedUserId;
        task.Title = updateTaskDto.Title;
        task.Description = updateTaskDto.Description;        
        task.Status = updateTaskDto.Status;
        task.DueDate = updateTaskDto.DueDate;
        task.ModifiedByUserId = userId;

        var updatedTask = await taskRepository.UpdateAsync(task);

        return OperationResult.Success(updatedTask.Adapt<TaskDto>());

    }

    public async Task<OperationResult> GetByProjectAsync(int projectId, int userId)
    {
        if((await projectRepository.Exist(projectId)) is false)
        {
            return OperationResult.Failure(OperationErrors.ProjectNotFound(projectId));
        }

        var tasks = await taskRepository.GetAsync(t => t.ProjectId == projectId);

        return OperationResult.Success(tasks.Adapt<IEnumerable<TaskDto>>());

    }


    protected async Task<bool> IsValidUser(int userId) =>
        await userRepository.Exist(userId);

    protected bool IsValidTask(ProjectTask task) =>
        task != null && task.DeletedAt == null;    
}