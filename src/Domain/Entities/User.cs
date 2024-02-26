namespace Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; set; }
    public string ProfileName { get; set; }
    public ICollection<Project> Projects { get; set; }
    public ICollection<ProjectTask> AssignedTasks { get; set; }    
    public ICollection<ProjectTaskAuditTrail> TaskAuditTrails { get; set; }

    public User(string name, string profileName)
    {
        Name = name;
        ProfileName = profileName;
    }
}

public static class UserProfiles
{   
    public static string Manager => "manager";
    public static string TeamMember => "teamMember";
    public static string Enginer => "enginer";
    public static string QAEnginer => "qaEnginer";
    public static string ScrumMaster => "scrumMaster";
    public static string Customer => "customer";
}