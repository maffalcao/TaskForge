using Domain.Dtos;
using Domain.ErrorHandling;

namespace Domain.Interfaces.Services;
public interface ITaskService
{
    Task<OperationResult> AddAsync(AddTaskDto addTaskDto, int projectId, int userId);    

}