using Api.Validators;
using Domain.Dtos;
using Domain.Interfaces.Persistence;
using Domain.Interfaces.Services;
using Domain.Services;
using FluentValidation;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IUserService, UserService>();
        services.AddTransient<IValidator<AddProjectDto>, AddProjectDtoValidator>();
        services.AddTransient<IValidator<AddTaskDto>, AddTaskDtoValidator>();
    }

    public static void AddInfraestructureServices(this IServiceCollection services, IConfiguration configuration)
    {        
        services.AddDbContext<ApplicationContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("TaskForgeDbConnectionString")));

        services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IProjectRepository, ProjectRepository>();

    }
}
