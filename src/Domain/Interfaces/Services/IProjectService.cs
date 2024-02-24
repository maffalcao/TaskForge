using Domain.Dtos;
using Domain.ErrorHandling;

namespace Domain.Interfaces.Services;

public interface IProjectService
{
    Task<OperationResult> AddAsync(AddProjectDto projectDto, int userId);
    Task<OperationResult> GetAllByUserIdAsync(int userId);
    Task<ProjectDto> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id);

    Task<OperationResult> AddProjectTaskAsync(AddTaskDto addProjectTaskDto, int projectId, int userId);
    Task<OperationResult> GetTasksAsync(int projectId, int userId);
}
