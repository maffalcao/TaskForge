using Domain.Dtos;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;

namespace Tests.IntegrationTests.Api;
public class GetProjectTasksActionIntegrationTests : BaseIntegrationTests, IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetProjectTasks_MissingUserId_ReturnsBadRequest()
    {
        // Arrange
        var requestUserId = 1;

        var application = new WebApplicationFactory();
        var client = application.CreateClient();

        var newProject = await InsertProject(requestUserId, application);
        var newTask = await InsertTask(newProject.Id, application);

        // Act
        var response = await client.GetAsync($"/project/{newProject.Id}/task/");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetProjectTasks_ValidRequest_ReturnsSuccess()
    {
        // Arrange        

        var application = new WebApplicationFactory();
        var client = application.CreateClient();

        var requestUser = InsertUser(application);
        var user2 = InsertUser(application);

        var newProject1 = await InsertProject(requestUser.Id, application);
        var newProject2 = await InsertProject(user2.Id, application);              

        var newTask1 = await InsertTask(newProject1.Id, application);
        var newTask2 = await InsertTask(newProject1.Id, application);
        var newTask3 = await InsertTask(newProject1.Id, application);
        var newTask4 = await InsertTask(newProject2.Id, application);

        client.DefaultRequestHeaders.Add("UserId", requestUser.Id.ToString());

        // Act
        var response = await client.GetAsync($"/project/{newProject1.Id}/task/");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();

        var operationResult = JsonConvert.DeserializeObject<Response<IEnumerable<TaskDto>>>(responseContent);

        var tasks = operationResult.Result.ToList();      

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        tasks.Should().NotBeEmpty();
        tasks.Count.Should().Be(3);
        tasks[0].Title.Should().Be(newTask1.Title);
        tasks[0].Id.Should().Be(newTask1.Id);
        tasks[1].Title.Should().Be(newTask2.Title);
        tasks[1].Id.Should().Be(newTask2.Id);
        tasks[2].Title.Should().Be(newTask3.Title);
        tasks[2].Id.Should().Be(newTask3.Id);
    }
}
