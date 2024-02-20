using Domain.Dtos;

namespace Domain.ErrorHandling;

public class OperationResult
{
    public IDto Result { get; private set; }
    public bool IsSuccess { get; private set; }
    public bool IsFailure
    {
        get
        {
            return !IsSuccess;
        }
    }

    public string ErrorMessage { get; private set; }

    public OperationResult() { } // to json conversion
    protected OperationResult(bool isSuccess, string errorMessage, IDto dto = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        Result = dto;
    }


    public static OperationResult Success(IDto dto) => new OperationResult(true, null, dto);
    public static OperationResult Failure(string message) => new OperationResult(false, message);
}

public record Error(string Type, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);
}

public static class Errors
{
    public static readonly Error UserNotFoundException = new Error(
        "Project.AddNewProject", "User not found");
}