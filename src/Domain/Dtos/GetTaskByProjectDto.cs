using Domain.Entities;

namespace Domain.Dtos;
public class GetTaskByProjectDto : IDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime? DueDate { get; set; }
    public ProjectTaskStatus Status { get; set; }
    public ProjectTaskPriority Priority { get; set; }
    public int? AssignedUserId { get; set; }
    public int ProjectId { get; set; }

    public List<TaskCommentDto> Comments { get; set; }
}

public class TaskCommentDto
{
    public int UserId { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedDate { get; set; }

    public TaskCommentDto(int userId, string comment, DateTime createdDate)
    {
        UserId = userId;
        Comment = comment;
        CreatedDate = createdDate;
    }
}