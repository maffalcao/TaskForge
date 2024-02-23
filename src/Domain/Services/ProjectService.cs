using Domain.Dtos;
using Domain.Entities;
using Domain.ErrorHandling;
using Domain.Interfaces.Persistence;
using Domain.Interfaces.Services;
using Mapster;

namespace Domain.Services;

public class ProjectService(IProjectRepository projectRepository, IRepository<User> userRepository) : IProjectService
{
    public async Task<OperationResult> AddAsync(AddProjectDto projectDto, int userId)
    {
        if ((await UserExist(userId)) is false)
        {
            return OperationResult.Failure(Errors.UserNotFound(userId));            
        }

        var project = new Project(projectDto.Name, userId);
        project = await projectRepository.AddProjectAsync(project);

        return OperationResult.Success(project.Adapt<ProjectDto>());

    }
    public async Task<bool> DeleteAsync(int id)
    {
        return await projectRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<ProjectDto>> GetAllByUserIdAsync(int userId)
    {
        var projects = await projectRepository.GetAllByUserId(userId);

        return projects.Adapt<IEnumerable<ProjectDto>>();
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
            return OperationResult.Failure(Errors.UserNotFound(userId));
        }

        if(addProjectTaskDto.AssignedUserId is not null)
        {
            var assignUserId = (int)addProjectTaskDto.AssignedUserId;
            if ((await UserExist(assignUserId) is false))
            {
                return OperationResult.Failure(Errors.UserNotFound(assignUserId));
            }
        }

        var project = await projectRepository.GetByIdAsync(projectId);

        if (project is null)
        {
            return OperationResult.Failure(Errors.ProjectNotFound(projectId));
        }

        var task = addProjectTaskDto.Adapt<ProjectTask>();
        task.ProjectId = projectId;        

        if(project.AddTask(task) is false)
        {
            return OperationResult.Failure(Errors.ProjectMaxNumberOfTasksAchieved(projectId));
        }

        await projectRepository.UpdateAsync(project);

        return OperationResult.Success();

    }

    private async Task<bool> UserExist(int userId) =>
        await userRepository.Exist(userId);

    
}