
using Domain.Entities;

namespace Domain.Dtos;

public class UpdateTaskDto : IDto
{    
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime? DueDate { get; set; }
    public ProjectTaskStatus Status { get; set; }
    public ProjectTaskPriority Priority { get; set; }    
    public int? AssignedUserId { get; set; }    
}
