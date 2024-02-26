using Domain.Dtos;
using Domain.Entities;
using Domain.ErrorHandling;
using Domain.Interfaces.Persistence;
using Domain.Interfaces.Services;
using Mapster;

namespace Domain.Services;

public class TaskService(
    IRepository<Project> projectRepository, 
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
        if ((await userRepository.Exist(userId)) is false)
        {
            return OperationResult.Failure(OperationErrors.UserNotFound(userId));
        }

        if (addTaskDto.AssignedUserId != null && (await userRepository.Exist(userId)) is false)
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

        if ((await userRepository.Exist(userId)) is false)
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

        if ((await userRepository.Exist(userId)) is false)
        {
            return OperationResult.Failure(OperationErrors.UserNotFound(userId));
        }

        if (updateTaskDto.AssignedUserId != null && (await userRepository.Exist(userId)) is false)
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
        task.SetStatus(updateTaskDto.Status);
        task.DueDate = updateTaskDto.DueDate;
        task.ModifiedByUserId = userId;

        task.CompletedAt = task.Status == ProjectTaskStatus.Done ? DateTime.UtcNow : null;

        var updatedTask = await taskRepository.UpdateAsync(task);

        return OperationResult.Success(updatedTask.Adapt<TaskDto>());

    }    

    public async Task<OperationResult> AddComment(AddTaskCommentDto commentDto, int taskId, int userId)
    {
        var task = await taskRepository.GetByIdAsync(taskId);

        if (!IsValidTask(task))
        {
            return OperationResult.Failure(OperationErrors.TaskNotFound(taskId));
        }

        if((await userRepository.Exist(userId)) is false)
        {
            return OperationResult.Failure(OperationErrors.UserNotFound(userId));
        }

        task.Comment = commentDto.Comment;
        task.ModifiedByUserId = userId;

        await taskRepository.UpdateAsync(task);

        return OperationResult.Success(commentDto);

    }

    public async Task<OperationResult> GetTasksDoneByUsers(int userId, int daysToConsider = 30)
    {
        var user = await userRepository.GetByIdAsync((int)userId);

        if(user == null) {
            return OperationResult.Failure(OperationErrors.UserNotFound(userId));
        }

        if(user.ProfileName != UserProfiles.Manager)
        {
            return OperationResult.Failure(OperationErrors.UserNotAllowed(userId));
        }

        var tasks = await taskRepository.GetAsync(t =>
            t.AssignedUserId != null &&
            t.Status == ProjectTaskStatus.Done &&
            t.DeletedAt == null &&
            t.CompletedAt >= DateTime.Now.AddDays(-daysToConsider)

            , includeString: "AssignedUser");

        var tasksByUsers = tasks
            .GroupBy(t => new { t.AssignedUser.Id, t.AssignedUser.Name })
            .Select(t => new GetTasksDoneByUsersDto(t.Key.Id, t.Key.Name, t.Count()));

        return OperationResult.Success(tasksByUsers);

    }




    protected bool IsValidTask(ProjectTask task) =>
        task != null && task.DeletedAt == null;
}