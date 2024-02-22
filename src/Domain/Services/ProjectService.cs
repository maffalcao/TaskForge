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
        if (await userRepository.Exist(userId))
        {
            var project = new Project(projectDto.Name, userId);
            project = await projectRepository.AddProjectAsync(project);

            return OperationResult.Success(project.Adapt<ProjectDto>());
        }


        return OperationResult.Failure(Errors.UserNotFoundException.Description);

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
}