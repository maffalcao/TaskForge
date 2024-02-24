using Domain.Dtos;
using Domain.ErrorHandling;

namespace Domain.Interfaces.Services;
public interface ITaskService
{
    Task<OperationResult> AddAsync(AddTaskDto addTaskDto, int projectId, int userId);
    Task<OperationResult> UpdateAsync(UpdateTaskDto updateTaskDto, int taskId, int userId);
    Task<OperationResult> DeleteAsync(int taskId, int userId);

    Task<OperationResult> GetByProjectAsync(int projectId, int userId);

}