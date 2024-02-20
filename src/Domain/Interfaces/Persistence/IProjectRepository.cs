using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Interfaces.Persistence;

public interface IProjectRepository : IRepository<Project>
{
    Task<IEnumerable<Project>> GetAllByUserId(int userId);
    Task<Project> AddProjectAsync(Project project);
}