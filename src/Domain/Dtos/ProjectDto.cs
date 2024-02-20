namespace Domain.Dtos;

public class ProjectDto: IDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int CreatedByUserId { get; set; }

    public UserDto CreatedByUser { get; set; }
}