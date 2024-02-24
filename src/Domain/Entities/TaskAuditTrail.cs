namespace Domain.Entities;
public class ProjectTaskAuditTrail : BaseEntity
{
    public string ChangedField { get; set; }
    public string PreviousValue { get; set; }
    public string NewValue { get; set; }
    public DateTime CreatedAt { get; set; }
    public int TaskId { get; set; }
    public int ModifiedByUserId { get; set; }
    public User ModifiedByUser { get; set; }
    public ProjectTask Task { get; set; }
}
