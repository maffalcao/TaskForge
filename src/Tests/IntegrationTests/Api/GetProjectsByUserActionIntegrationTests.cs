using Domain.Dtos;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace Tests.IntegrationTests.Api;
public class GetProjectsByUserActionIntegrationTests: BaseIntegrationTests, 
    IClassFixture<WebApplicationFactory<Program>>
{

    [Fact]
    public async Task GetProjectsByUser_MissingHeaderUserId_ReturnsBadRequest()
    {
        // Arrange
        const int projectsFromUserId = 1;
        
        var application = new WebApplicationFactory();
        var client = application.CreateClient();


        // Act
        var response = await client.GetAsync($"/project/{projectsFromUserId}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetProjectsByUser_InvalidUserId_ReturnsNotFound()
    {
        // Arrange

        const int requestUserId = 1;
        const int projectsFromUserId = 100;

        var application = new WebApplicationFactory();
        var client = application.CreateClient();
        client.DefaultRequestHeaders.Add("UserId", requestUserId.ToString());


        // Act
        var response = await client.GetAsync($"/project/{projectsFromUserId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProjectsByUserId_ValidUserId_ReturnsItsProjects()
    {
        var application = new WebApplicationFactory();
        var client = application.CreateClient();

        var user1 = await InsertUser(application);
        var user2 = await InsertUser(application); 

        var project1 = await InsertProject(user1.Id, application);
        var project2 = await InsertProject(user1.Id, application);
        var project3 = await InsertProject(user1.Id, application);
        await InsertProject(user2.Id, application);
        await InsertProject(user2.Id, application);

        client.DefaultRequestHeaders.Add("userId", user1.Id.ToString());

        // Act
        var response = await client.GetAsync($"/project/{user1.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();

        var operationResult = JsonConvert.DeserializeObject<Response<IEnumerable<ProjectDto>>>(responseContent);

        var projects = operationResult.Result.ToList();

        projects.Should().NotBeEmpty();
        projects.Count.Should().Be(3);
        projects[0].Name.Should().Be(project1.Name);
        projects[1].Name.Should().Be(project2.Name);
        projects[2].Name.Should().Be(project3.Name);

        projects[0].CreatedByUserId.Should().Be(user1.Id);
        projects[1].CreatedByUserId.Should().Be(user1.Id);
        projects[2].CreatedByUserId.Should().Be(user1.Id);

    }

}
