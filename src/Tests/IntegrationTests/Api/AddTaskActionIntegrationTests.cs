using Bogus;
using Domain.Dtos;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace Tests.IntegrationTests.Api;
public class AddTaskActionIntegrationTests: BaseIntegrationTests, IClassFixture<WebApplicationFactory<Program>>
{    
    [Fact]
    public async Task AddTask_MissingUserId_ReturnsBadRequest()
    {
        // Arrange
        var requestUserId = 1;

        var application = new WebApplicationFactory();
        var client = application.CreateClient();
        var faker = new Faker();
        var newProject = await InsertProject(requestUserId, application);

        var newTask = GetAddTaskDto();

        var content = Serialize(newTask);

        // Act
        var response = await client.PostAsync($"/project/{newProject.Id}/task", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddTask_MissingTaskTitle_ReturnsBadRequest()
    {
        // Arrange
        var requestUserId = 1;
        var faker = new Faker();

        var application = new WebApplicationFactory();

        var client = application.CreateClient();
        client.DefaultRequestHeaders.Add("UserId", requestUserId.ToString());
                
        var newProject = await InsertProject(requestUserId, application);

        var newTask = GetAddTaskDto();
        
        newTask.Title = null;

        var content = Serialize(newTask);

        // Act
        var response = await client.PostAsync($"/project/{newProject.Id}/task", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddTask_MissingTaskDescription_ReturnsBadRequest()
    {
        // Arrange
        var requestUserId = 1;
        var faker = new Faker();

        var application = new WebApplicationFactory();

        var client = application.CreateClient();
        client.DefaultRequestHeaders.Add("UserId", requestUserId.ToString());

        var newProject = await InsertProject(requestUserId, application);

        var newTask = GetAddTaskDto();

        newTask.Description = null;

        var content = new StringContent(JsonConvert.SerializeObject(newTask), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync($"/project/{newProject.Id}/task", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddTask_ValidTask_ReturnsSuccess()
    {
        // Arrange
        var requestUserId = 1;        

        var application = new WebApplicationFactory();

        var client = application.CreateClient();
        client.DefaultRequestHeaders.Add("UserId", requestUserId.ToString());

        var newProject = await InsertProject(requestUserId, application);

        var newTaskDto = GetAddTaskDto();

        var content = Serialize(newTaskDto);

        // Act
        var response = await client.PostAsync($"/project/{newProject.Id}/task", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();

        var operationResult = JsonConvert.DeserializeObject<Response<TaskDto>>(responseContent);
        var taskDtoCreated = operationResult.Result;

        taskDtoCreated.Should().NotBeNull();
        taskDtoCreated.ProjectId.Should().Be(newProject.Id);
        taskDtoCreated.Title.Should().Be(newTaskDto.Title);
        taskDtoCreated.Description.Should().Be(newTaskDto.Description);
        taskDtoCreated.Status.Should().Be(newTaskDto.Status);
        taskDtoCreated.Priority.Should().Be(newTaskDto.Priority);
    }
}
