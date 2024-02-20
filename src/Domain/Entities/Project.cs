namespace Domain.Entities;
public class Project : BaseEntity
{
    public string Name { get; set; }
    public int CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; }

    public Project(string name, int createdByUserId)
    {
        Name = name;
        CreatedByUserId = createdByUserId;        
    }

}