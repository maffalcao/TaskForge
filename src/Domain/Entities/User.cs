namespace Domain.Entities;

public class User: BaseEntity
{    
    public string Name { get; set; }
    public string ProfileName { get; set; }
    public ICollection<Project> Projects { get; set; }

    public User(string name, string profileName)
    {
        Name = name;
        ProfileName = profileName;        
    }
}