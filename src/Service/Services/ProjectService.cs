using AutoMapper;
using Domain.Dtos;
using Domain.Entities;
using Domain.Interfaces.Persistence;
using Domain.Interfaces.Services;

namespace Service.Services;
public class ProjectService(IProjectRepository repository, IMapper mapper) : IProjectService
{
    public async Task<ProjectReponseDto> AddAsync(AddProjectRequestDto projectDto, int userId)
    {
        var project = new Project(projectDto.Name, userId);        
        
        project = await repository.AddAsync(project);
        
        return mapper.Map<ProjectReponseDto>(project);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<ProjectReponseDto>> GetAllByUserIdAsync(int userId)
    {
        var projects = await repository.GetAllByUserId(userId);

        return mapper.Map<IEnumerable<ProjectReponseDto>>(projects);
    }

    public async Task<ProjectReponseDto> GetByIdAsync(int id)
    {
        var project = await repository.GetByIdAsync(id);
        
        return mapper.Map<ProjectReponseDto>(project);
    }


}