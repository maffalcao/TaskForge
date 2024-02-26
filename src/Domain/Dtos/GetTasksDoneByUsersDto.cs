
namespace Domain.Dtos;
public class GetTasksDoneByUsersDto
{
    public int UserId { get; set; }
    public string UserName { get; set; }
    public int NumberOfTasksCompleted { get; set; }

    public GetTasksDoneByUsersDto(int userId, string userName, int numberOfTasksCompleted)
    {
        UserId = userId;
        UserName = userName;
        NumberOfTasksCompleted = numberOfTasksCompleted;
    }
}
