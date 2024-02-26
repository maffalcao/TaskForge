
using Bogus;
using Domain.Dtos;
using FluentAssertions;
using Mapster;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace Tests.IntegrationTests.Api;
public class UpdateTaskActionIntegrationController : BaseIntegrationTests, IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task UpdateTask_MissingUserId_ReturnsBadRequest()
    {
        // Arrange
        var requestUserId = 1;

        var application = new WebApplicationFactory();
        var client = application.CreateClient();        

        var newProject = await InsertProject(requestUserId, application);
        var newTask = await InsertTask(newProject.Id, application);

        var updateTaskDto = newTask.Adapt<UpdateTaskDto>();

        updateTaskDto.Description = _faker.Lorem.Text();

        var content = Serialize(updateTaskDto);

        // Act
        var response = await client.PostAsync($"/project/{newProject.Id}/task/{newTask.Id}", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateTask_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var requestUserId = 1;

        var application = new WebApplicationFactory();
        var client = application.CreateClient();

        client.DefaultRequestHeaders.Add("UserId", requestUserId.ToString());        

        var newProject = await InsertProject(requestUserId, application);
        var newTask = await InsertTask(newProject.Id, application);

        var updateTaskDto = newTask.Adapt<UpdateTaskDto>();

        updateTaskDto.Description = _faker.Lorem.Text();

        var content = Serialize(updateTaskDto);

        // Act
        var response = await client.PutAsync($"/project/{newProject.Id}/task/{newTask.Id}", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadAsStringAsync();
        var operationResult = JsonConvert.DeserializeObject<Response<TaskDto>>(responseContent);
        var taskDtoCreated = operationResult.Result;

        taskDtoCreated.Should().NotBeNull();
        taskDtoCreated.Id.Should().Be(newTask.Id);
        taskDtoCreated.ProjectId.Should().Be(newTask.ProjectId);
        taskDtoCreated.Title.Should().Be(updateTaskDto.Title);        
        taskDtoCreated.Status.Should().Be(updateTaskDto.Status);
        taskDtoCreated.Priority.Should().Be(updateTaskDto.Priority);

        taskDtoCreated.Description.Should().Be(updateTaskDto.Description);
    }

}
