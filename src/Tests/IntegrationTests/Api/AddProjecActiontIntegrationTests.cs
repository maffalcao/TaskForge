using Domain.Dtos;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;
using System.Text;

namespace Tests.IntegrationTests.Api;
public class AddProjecActiontIntegrationTests: BaseIntegrationTests, IClassFixture<WebApplicationFactory<Program>>
{    

    [Fact]
    public async Task AddProject_MissingUserId_ReturnsBadRequest()
    {
        // Arrange
        var application = new WebApplicationFactory();
        var client = application.CreateClient();

        var newProjectDto = GetAddProjectDto();

        var content = Serialize(newProjectDto);

        // Act
        var response = await client.PostAsync("/project", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddProject_NullOrEmptyName_ReturnsBadRequest()
    {
        // Arrange
        var application = new WebApplicationFactory();
        var client = application.CreateClient();        

        var newProjectDto = GetAddProjectDto();
        newProjectDto.Name = null;

        var content = Serialize(newProjectDto);

        // Act
        var response = await client.PostAsync("/project", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddNewProject_ValidProject_ReturnsSuccess()
    {
        // Arrange
        const int requestUserId = 1;
        var application = new WebApplicationFactory();
        var client = application.CreateClient();

        client.DefaultRequestHeaders.Add("userId", requestUserId.ToString());
        var newProjectDto = GetAddProjectDto();

        var content = Serialize(newProjectDto);

        // Act
        var response = await client.PostAsync("/project", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();        

        var operationResult = JsonConvert.DeserializeObject<Response<ProjectDto>>(responseContent);
        var projectDtoCreated = operationResult.Result;

        projectDtoCreated.Should().NotBeNull();
        projectDtoCreated.Name.Should().Be(newProjectDto.Name);
        projectDtoCreated.CreatedByUserId.Should().Be(requestUserId);
    }


}
