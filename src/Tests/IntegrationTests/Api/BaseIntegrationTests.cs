using Bogus;
using Bogus.DataSets;
using Domain.Dtos;
using Domain.Entities;
using Domain.ErrorHandling;
using Infrastructure.Context;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Text;

namespace Tests.IntegrationTests.Api;
public class BaseIntegrationTests
{
    protected readonly Faker _faker;
    public BaseIntegrationTests()
    {
        _faker = new Faker();
    }

    public class Response<T> where T : class
    {
        public T Result { get; set; }
        public bool IsSuccess { get; set; }
        public List<Error> Errors { get; set; }
    }

    protected async Task EmptyTableAsync<T>(DbSet<T> table, ApplicationContext context) where T : class
    {
        var allRecords = await table.ToListAsync();
        table.RemoveRange(allRecords);
        await context.SaveChangesAsync();
    }

    protected StringContent Serialize(object obj)
    {
        return new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
    }

    internal async Task<Project> InsertProject(int userId, 
        WebApplicationFactory webApplicationFactory, bool emptyTableBefore = false)
    {
        using var scope = webApplicationFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

        if (emptyTableBefore)
        {
            await EmptyTableAsync(dbContext.Projects, dbContext);
        }

        var toAddProject = GetAddProjectDto().Adapt<Project>();
        toAddProject.CreatedByUserId = userId;
            

        dbContext.Projects.Add(toAddProject);
        await dbContext.SaveChangesAsync();

        return dbContext.Projects.OrderByDescending(p => p.Id).First();        
    }

    internal async Task<ProjectTask> InsertTask(int projectId,
        WebApplicationFactory webApplicationFactory, bool emptyTableBefore = false)
    {
        using var scope = webApplicationFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

        if (emptyTableBefore)
        {
            await EmptyTableAsync(dbContext.Projects, dbContext);
        }

        var toAddTask = GetAddTaskDto().Adapt<ProjectTask>();
        toAddTask.ProjectId = projectId;   

        dbContext.Tasks.Add(toAddTask);
        await dbContext.SaveChangesAsync();

        return dbContext.Tasks.OrderByDescending(p => p.Id).First();
    }

    internal async Task<User> InsertUser(WebApplicationFactory webApplicationFactory)
    {
        using var scope = webApplicationFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

        var user = new User(_faker.Name.FullName(), UserProfiles.TeamMember);        

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        return dbContext.Users.OrderByDescending(p => p.Id).First();
    }



    protected AddProjectDto GetAddProjectDto()
    {
        return new AddProjectDto() { Name = _faker.Lorem.Sentence(5)};
    }

    protected AddTaskDto GetAddTaskDto()
    {        

        return new AddTaskDto()
        {
            Title = _faker.Lorem.Sentence(5),
            Description = _faker.Lorem.Sentence(10),
            Status = _faker.PickRandom<ProjectTaskStatus>(),
            Priority = _faker.PickRandom<ProjectTaskPriority>()
        };
    }
}
