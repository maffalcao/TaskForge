using Domain.Dtos;
using FluentAssertions;
using Mapster;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;

namespace Tests.IntegrationTests.Api;
public class DeleteTaskActionIntegrationTests : BaseIntegrationTests, IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task DeleteTask_MissingUserId_ReturnsBadRequest()
    {
        // Arrange
        var requestUserId = 1;

        var application = new WebApplicationFactory();
        var client = application.CreateClient();

        var newProject = await InsertProject(requestUserId, application);
        var newTask = await InsertTask(newProject.Id, application);


        // Act
        var response = await client.DeleteAsync($"/project/{newProject.Id}/task/{newTask.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteTask_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var requestUserId = 1;

        var application = new WebApplicationFactory();

        var client = application.CreateClient();
        client.DefaultRequestHeaders.Add("UserId", requestUserId.ToString());

        var newProject = await InsertProject(requestUserId, application);
        var newTask = await InsertTask(newProject.Id, application);

        // Act
        var response = await client.DeleteAsync($"/project/{newProject.Id}/task/{newTask.Id}");

        var responseContent = await response.Content.ReadAsStringAsync();
        var operationResult = JsonConvert.DeserializeObject<Response<TaskDto>>(responseContent);
        var deletedTask = operationResult.Result;

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        newTask.DeletedAt.Should().BeNull();
        deletedTask.DeletedAt.Should().NotBeNull();
    }
}