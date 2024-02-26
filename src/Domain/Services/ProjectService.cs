﻿using Domain.Dtos;
using Domain.Entities;
using Domain.ErrorHandling;
using Domain.Interfaces.Persistence;
using Domain.Interfaces.Services;
using Mapster;

namespace Domain.Services;

public class ProjectService(
    IRepository<Project> projectRepository, 
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

        var addedProject = await projectRepository.AddAsync(project);

        var projects = await projectRepository.GetAsync(p => 
            p.Id == addedProject.Id, includeString: "CreatedByUser");
        
        return OperationResult.Success(projects.FirstOrDefault().Adapt<ProjectDto>());

    }
    public async Task<OperationResult> DeleteAsync(int projectId, int userId)
    {
        var project = await GetProject(projectId, includeString: "Tasks");

        if (project is null)
        {
            return OperationResult.Failure(OperationErrors.ProjectNotFound(projectId));
        }

        if ((await UserExist(userId)) is false)
        {
            return OperationResult.Failure(OperationErrors.UserNotFound(userId));
        }

        var pendintTaskStatus = new List<ProjectTaskStatus>() {
                ProjectTaskStatus.InProgress,
                ProjectTaskStatus.Pending
            };          
            

        if(project.Tasks != null && project.Tasks.Any(t => pendintTaskStatus.Contains(t.Status))) {
        
            return OperationResult.Failure(OperationErrors.ProjectHavePendingTasks(projectId));
        }

        project.DeletedAt = DateTime.UtcNow;
        await projectRepository.UpdateAsync(project);

        return OperationResult.Success(); 
        
    }

    public async Task<OperationResult> GetAllByUserIdAsync(int userId)
    {
        if ((await UserExist(userId)) is false)
        {
            return OperationResult.Failure(OperationErrors.UserNotFound(userId));
        }
        
        var projects = await projectRepository.GetAsync(p => 
            p.CreatedByUserId == userId && p.DeletedAt == null, includeString: "CreatedByUser");

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
            t => t.ProjectId == projectId && t.DeletedAt == null, includeString: "AuditTrails");

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


    private async Task<Project> GetProject(int projectId, string includeString)
    {
        var projects =
            await projectRepository.GetAsync(
                p => p.Id.Equals(projectId), includeString: includeString);

        return projects.FirstOrDefault();
    }

    private async Task<bool> UserExist(int userId) =>
        await userRepository.Exist(userId);   
}