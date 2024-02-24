namespace Domain.Entities;
public class ProjectTask: BaseEntity
{ 
    public string Title { get; set; }
    public string Description { get; set; }    
    public DateTime? DueDate { get; set; }
    public ProjectTaskStatus Status { get; set; }
    public ProjectTaskPriority Priority { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? AssignedUserId { get; set; }
    public int ProjectId { get; set; }        
    public User AssignedUser { get; set; }
    public Project Project { get; set; }
    public ICollection<ProjectTaskAuditTrail> AuditTrails { get; set; }
}

public enum ProjectTaskStatus
{
    Pending = 1,
    InProgress,
    Done
}

public enum ProjectTaskPriority
{
    Low = 1,
    Medium = 2,
    High = 3
}
