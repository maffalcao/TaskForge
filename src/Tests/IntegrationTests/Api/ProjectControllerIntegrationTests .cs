using System.Net;
using System.Text;
using Domain.Dtos;
using Domain.Entities;
using Domain.ErrorHandling;
using Infrastructure.Context;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Tests.IntegrationTests;
using static System.Net.Mime.MediaTypeNames;


public class ProjectControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>> 
{     

    [Fact]
    public async Task AddNewProject_ValidProject_ReturnsSuccess()
    {
        // Arrange
        const int requestUserId = 1;
        var application = new WebApplicationFactory();
        var client = application.CreateClient();

        client.DefaultRequestHeaders.Add("userId", requestUserId.ToString());


        var newProject = new AddProjectDto { Name = "Test Project" };
        var content = new StringContent(JsonConvert.SerializeObject(newProject), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/project", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var projectDtoCreated = JsonConvert.DeserializeObject<ProjectDto>(responseContent);      
        
        Assert.NotNull(projectDtoCreated);
        Assert.Equal(newProject.Name, projectDtoCreated.Name);
        Assert.Equal(requestUserId, projectDtoCreated.CreatedByUserId);
    }

    [Fact]
    public async Task AddNewProject_InvalidUserId_ReturnsBadRequest()
    {
        // Arrange
        const int nonExistentUserId = 999; 
        var application = new WebApplicationFactory();
        var client = application.CreateClient();

        client.DefaultRequestHeaders.Add("userId", nonExistentUserId.ToString());

        var newProject = new AddProjectDto { Name = "Test Project" };
        var content = new StringContent(JsonConvert.SerializeObject(newProject), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/project", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetProjectsByUserId_ValidUserId_ReturnsItsProjects()
    {        
        var application = new WebApplicationFactory();
        var client = application.CreateClient();
       

        var userId1 = 1;
        var userId2 = 2;

        await InsertProject("projeto1", userId1, application, emptyTableBefore: true);
        await InsertProject("projeto2", userId1, application);
        await InsertProject("projeto3", userId1, application);
        await InsertProject("projeto4", userId2, application);
        await InsertProject("projeto4", userId2, application);
        

        client.DefaultRequestHeaders.Add("userId", userId1.ToString());

        // Act
        var response = await client.GetAsync($"/project/{userId1}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var projects = JsonConvert.DeserializeObject<IEnumerable<ProjectDto>>(responseContent);

        Assert.NotEmpty(projects);
        Assert.True(projects.Count() == 3);
        Assert.True(projects.ElementAt(0).Name == "projeto1");
        Assert.True(projects.ElementAt(1).Name == "projeto2");
        Assert.True(projects.ElementAt(2).Name == "projeto3");

    }

    internal async Task InsertProject(string name, int userId, WebApplicationFactory webApplicationFactory, bool emptyTableBefore = false)
    {
        using var scope = webApplicationFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

        if(emptyTableBefore)
        {
            await EmptyTableAsync<Project>(dbContext.Projects, dbContext);
        }

        var project = new Project(name, userId);
        
        dbContext.Projects.Add(project);
        await dbContext.SaveChangesAsync();

    }

    private async Task EmptyTableAsync<T>(DbSet<T> table, ApplicationContext context) where T : class
    {
        var allRecords = await table.ToListAsync();
        table.RemoveRange(allRecords);
        await context.SaveChangesAsync();
    }


}
