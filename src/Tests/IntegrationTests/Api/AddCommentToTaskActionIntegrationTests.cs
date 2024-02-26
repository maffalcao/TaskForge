using Domain.Dtos;
using FluentAssertions;
using Infrastructure.Context;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;

namespace Tests.IntegrationTests.Api;
public class AddCommentToTaskActionIntegrationTests : BaseIntegrationTests, IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task AddCommentToTask_MissingUserId_ReturnsBadRequest()
    {
        // Arrange
        var requestUserId = 1;

        var application = new WebApplicationFactory();
        var client = application.CreateClient();

        var newProject = await InsertProject(requestUserId, application);
        var newTask = await InsertTask(newProject.Id, application);

        var commentDto = new AddTaskCommentDto() { Comment = _faker.Lorem.Sentence(6) };
        var content = Serialize(commentDto);

        // Act
        var response = await client.PostAsync($"/project/{newProject.Id}/task/{newTask.Id}/comment", content);        

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddCommentToTask_ValidRequest_ReturnsSuccess()
    {
        // Arrange                
        var requestUserId = 1;

        var application = new WebApplicationFactory();

        var client = application.CreateClient();
        client.DefaultRequestHeaders.Add("UserId", requestUserId.ToString());

        var newProject = await InsertProject(requestUserId, application);
        var newTask = await InsertTask(newProject.Id, application);

        var newCommentDto = new AddTaskCommentDto() { Comment = _faker.Lorem.Sentence(6) };
        var content = Serialize(newCommentDto);

        // Act
        var response = await client.PostAsync($"/project/{newProject.Id}/task/{newTask.Id}/comment", content);

        using var scope = application.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

        var insertedTaskAuditTrail = dbContext
            .TaskAuditTrails
            .Where(t => t.ChangedField == "Comment")
            .OrderByDescending(t => t.Id).First();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        insertedTaskAuditTrail.Should().NotBeNull();
        newTask.Id.Should().Be(insertedTaskAuditTrail.TaskId);        
        insertedTaskAuditTrail.NewValue.Should().Be(newCommentDto.Comment);
        
        
    }
}
