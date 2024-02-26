using Domain.Entities;

namespace Domain.Dtos;
public class TaskDto : IDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public ProjectTaskStatus Status { get; set; }
    public ProjectTaskPriority Priority { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? AssignedUserId { get; set; }
    public int ProjectId { get; set; }    
}
