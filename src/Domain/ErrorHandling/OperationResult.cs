using Domain.Dtos;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Domain.ErrorHandling;

public class OperationResult
{
    public IDto? Result { get; private set; }
    public bool IsSuccess { get; private set; }
    public bool IsFailure
    {
        get
        {
            return !IsSuccess;
        }
    }
    
    public Error? Error { get; private set; }

    public OperationResult(bool v) { } // to json conversion
    protected OperationResult(bool isSuccess, IDto dto = null, Error error = null)
    {
        IsSuccess = isSuccess;        
        Result = dto;
        Error = error;
        
    }

    public static OperationResult Success(IDto dto) => new OperationResult(true, dto);
    public static OperationResult Success() => new OperationResult(true);    
    public static OperationResult Failure(Error error) => new OperationResult(false, error: error);
}

public record Error(string Type, string Description, int StatusCode)
{
    public static readonly Error None = new(string.Empty, string.Empty, 0);
}

public static class Errors
{
    public static Error UserNotFound(int userId) => new Error(
        "Project.AddNewProject", 
        $"User {userId} not found", 
        StatusCodes.Status404NotFound);

    public static Error ProjectNotFound(int projectId) => new Error(
        "Project.AddNewTask", 
        $"Project {projectId} not found", 
        StatusCodes.Status404NotFound);

    public static Error ProjectMaxNumberOfTasksAchieved(int projectId) => new Error(
        "Project.AddNewTask", 
        $"Project {projectId}'s tasks limit (20) achieved. It's not possible to add another.",
        StatusCodes.Status409Conflict);
}