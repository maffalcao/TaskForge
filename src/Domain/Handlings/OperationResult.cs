using Microsoft.AspNetCore.Http;

namespace Domain.ErrorHandling;

public class OperationResult
{
    public Object? Result { get; private set; }
    public bool IsSuccess { get; private set; }    

    public List<Error>? Errors { get; private set; }

    public OperationResult(bool isSuccess) => IsSuccess = true;
    protected OperationResult(bool isSuccess, Object? result = null, List<Error>? errors = null)
    {
        IsSuccess = isSuccess;
        Result = result;

        if (!isSuccess && errors is not null)
        {
            Errors = [];
            Errors.AddRange(errors);
        }
        
    }

    public static OperationResult Failure(string type, string description, int httpStatusCode)
    {
        var error = new Error(type, description, httpStatusCode);
        return new OperationResult(false, errors: [error]);

    }

    public void SetResult(Object result) => Result = result;

    public static OperationResult Success(Object? result) => new OperationResult(true, result);
    public static OperationResult Success() => new OperationResult(true);    
    public static OperationResult Failure(Error error) => new OperationResult(false, errors: [error]);
    public static OperationResult Failure(List<Error> errors) => new OperationResult(false, errors: errors);

    public static OperationResult FromFluentValidationResponse(Dictionary<string, List<string>> validationErrors)
    {
        var errors = validationErrors.SelectMany(pair => pair.Value.Select(message => new Error(pair.Key, message, 400))).ToList();
        return Failure(errors);
    }

}

public record Error(string Type, string Description, int StatusCode)
{
    public static readonly Error None = new(string.Empty, string.Empty, 0);
}

public static class OperationErrors
{
    public static Error UserNotFound(int userId) => new Error(
        "Project.AddNewProject.UserNotFound", 
        $"User {userId} not found", 
        StatusCodes.Status404NotFound);

    public static Error UserNotFound() => new Error(
        "Project.Authentication.UserNotFound",
        $"User not found",
        StatusCodes.Status404NotFound);

    public static Error InvalidUser() => new Error(
        "Project.Authentication.InvalidUserId",
        $"User invalid",
        StatusCodes.Status404NotFound);

    public static Error ProjectNotFound(int projectId) => new Error(
        "Project.AddNewTask.ProjectNoutFound", 
        $"Project {projectId} not found", 
        StatusCodes.Status404NotFound);

    public static Error ProjectMaxNumberOfTasksAchieved(int projectId) => new Error(
        "Project.AddNewTask.TaskLimitAchieved", 
        $"Project {projectId}'s tasks limit (20) achieved. It's not possible to add another.",
        StatusCodes.Status409Conflict);
    public static Error TaskNotFound(int taskId) => new Error(
        "Task.UpdateTask.TaskNotFound",
        $"Task {taskId} not found",
        StatusCodes.Status404NotFound);
    public static Error TaskPriorityCantBechanged(int taskId) => new Error(
        "Task.UpdateTask.PriorityCantBechanged",
        $"Changing Task {taskId}' priority is not allowed",
        StatusCodes.Status409Conflict);

}