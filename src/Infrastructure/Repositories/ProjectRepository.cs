using Domain.Entities;
using Domain.Interfaces.Persistence;
using Infrastructure.Context;


namespace Infrastructure.Repositories;

public class ProjectRepository : BaseRepository<Project>, IProjectRepository
{
    public ProjectRepository(ApplicationContext context) : base(context)
    {
    }

    public async Task<Project> AddProjectAsync(Project project)
    {
        var addedProject = await AddAsync(project);

        var projects = await GetAsync(p => p.Id == addedProject.Id, includeString: "CreatedByUser");
        return projects.FirstOrDefault();

    }

    public async Task<IEnumerable<Project>> GetAllByUserId(int userId)
    {
        return await GetAsync(p => p.CreatedByUserId == userId, includeString: "CreatedByUser");
    }

}
