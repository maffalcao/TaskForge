using FluentAssertions;
using Infrastructure.Context;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Tests.IntegrationTests.Api;
public class DeleteProjectActionIntegrationTests : BaseIntegrationTests, IClassFixture<WebApplicationFactory<Program>>
{

    [Fact]
    public async Task DeleteProject_MissingUserId_ReturnsBadRequest()
    {
        // Arrange
        var requestUserId = 1;
        var application = new WebApplicationFactory();
        var client = application.CreateClient();

        var newProject = await InsertProject(requestUserId, application);

        // Act
        var response = await client.DeleteAsync($"/project/{newProject.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }    

    [Fact]
    public async Task DeleteProject_ValidProject_ReturnsSuccess()
    {
        // Arrange
        var requestUserId = 1;
        var application = new WebApplicationFactory();
        var client = application.CreateClient();
        client.DefaultRequestHeaders.Add("userId", requestUserId.ToString());

        var newProject = await InsertProject(requestUserId, application);

        // Act
        var response = await client.DeleteAsync($"/project/{newProject.Id}");

        using var scope = application.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        var deletedProject = dbContext.Projects.OrderByDescending(p => p.Id).FirstOrDefault();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);        
    }


}
