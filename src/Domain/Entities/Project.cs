using System.Threading.Tasks;

namespace Domain.Entities;
public class Project : BaseEntity
{
    private readonly int MAX_NUMBER_OF_TASKS = 20;
    public string Name { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime? DeletedAt { get; set; }
    public User CreatedByUser { get; set; }

    

    public ICollection<ProjectTask> Tasks { get; set; }

    public Project() {
        Tasks = new List<ProjectTask>();
    }

    public Project(string name, int createdByUserId)
    {
        Name = name;
        CreatedByUserId = createdByUserId;
    }

    public bool AddTask(ProjectTask task)
    {
        if (Tasks.Count == MAX_NUMBER_OF_TASKS)
            return false;

        Tasks.Add(task);
        return true;
    }

}