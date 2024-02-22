namespace Domain.Entities;
public class ProjectTask
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }    
    public DateTime DueDate { get; set; }
    public TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? AssignedUserId { get; set; }
    public int ProjectId { get; set; }        
    public User AssignedUser { get; set; }
    public Project Project { get; set; }
}

public enum TaskStatus
{
    Pending = 1,
    InProgress,
    Done
}

public enum TaskPriority
{
    Low = 1,
    Medium = 2,
    High = 3
}
