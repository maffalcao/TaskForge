using Domain.Dtos;
using Domain.ErrorHandling;

namespace Domain.Interfaces.Services;

public interface IProjectService
{
    Task<OperationResult> AddAsync(AddProjectDto projectDto, int userId);
    Task<IEnumerable<ProjectDto>> GetAllByUserIdAsync(int userId);
    Task<ProjectDto> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id);
}
