using Bogus;
using Domain.Dtos;
using Domain.Entities;
using Domain.ErrorHandling;
using Domain.Interfaces.Persistence;
using Domain.Services;
using FluentAssertions;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Collections.Generic;
using System.Linq.Expressions;

public class ProjectServiceTests
{   

    [Fact]
    public async Task AddAsync_WhenUserDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var projectDto = new AddProjectDto { Name = "Test Project" };
        var user = GetUser();

        var projectRepositoryMock = new Mock<IRepository<Project>>();

        var userRepositoryMock = new Mock<IRepository<User>>();

        userRepositoryMock.Setup(repo => repo.Exist(user.Id))
            .ReturnsAsync(false);

        var taskRepositoryMock = new Mock<IRepository<ProjectTask>>();
        var projectService = new ProjectService(
            projectRepositoryMock.Object,
            userRepositoryMock.Object,
            taskRepositoryMock.Object);

        // Act
        var result = await projectService.AddAsync(projectDto, user.Id);

        // Assert
        result.IsSuccess.Should().Be(false);

        var error = result.Errors.FirstOrDefault();

        error.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        error.Type.Should().Be(OperationErrors.UserNotFound(user.Id).Type);
    }    

    [Fact]
    public async Task AddProjectTaskAsync_UserNotFound_ReturnsFailure()
    {
        // Arrange
        var userRepositoryMock = new Mock<IRepository<User>>();
        var projectRepositoryMock = new Mock<IRepository<Project>>();
        var taskRepositoryMock = new Mock<IRepository<ProjectTask>>();

        userRepositoryMock.Setup(repo => 
            repo.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((User)null);

        var projectService = new ProjectService(
           projectRepositoryMock.Object,
           userRepositoryMock.Object,
           taskRepositoryMock.Object);

        // Act
        var result = await projectService.AddProjectTaskAsync(It.IsAny<AddTaskDto>(), It.IsAny<int>(), It.IsAny<int>());

        // Assert
        result.IsSuccess.Should().BeFalse();
        var error = result.Errors.FirstOrDefault();

        error.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        error.Type.Should().Be(OperationErrors.UserNotFound(It.IsAny<int>()).Type);
    }

    [Fact]
    public async Task AddProjectTaskAsync_ProjectNotFound_ReturnsFailure()
    {
        // Arrange
        var userRepositoryMock = new Mock<IRepository<User>>();
        var projectRepositoryMock = new Mock<IRepository<Project>>();
        var taskRepositoryMock = new Mock<IRepository<ProjectTask>>();
                
        var user = GetUser();
        var assTaskDto = new AddTaskDto();

        userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(user);

        projectRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Project) null);

        var projectService = new ProjectService(
           projectRepositoryMock.Object,
           userRepositoryMock.Object,
           taskRepositoryMock.Object);

        // Act
        var result = await projectService.AddProjectTaskAsync(assTaskDto, It.IsAny<int>(), It.IsAny<int>());

        // Assert
        result.IsSuccess.Should().BeFalse();
        var error = result.Errors.FirstOrDefault();

        error.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        error.Type.Should().Be(OperationErrors.ProjectNotFound(It.IsAny<int>()).Type);
    }

    [Fact]
    public async Task AddProjectTaskAsync_WhenMaxTasksReached_ReturnsFailure()
    {
        // Arrange
        var userRepositoryMock = new Mock<IRepository<User>>();
        var projectRepositoryMock = new Mock<IRepository<Project>>();
        var taskRepositoryMock = new Mock<IRepository<ProjectTask>>();
        
        var user = GetUser();
        var assTaskDto = new AddTaskDto();

        var project = new Project();

        for(int i = 0; i < 20; i++)
        {
            project.AddTask(new ProjectTask());
        }

        userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(user);

        projectRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(project);

        var projectService = new ProjectService(
           projectRepositoryMock.Object,
           userRepositoryMock.Object,
           taskRepositoryMock.Object);

        // Act
        var result = await projectService.AddProjectTaskAsync(assTaskDto, It.IsAny<int>(), It.IsAny<int>());

        // Assert
        result.IsSuccess.Should().BeFalse();
        var error = result.Errors.FirstOrDefault();

        error.StatusCode.Should().Be(StatusCodes.Status409Conflict);
        error.Type.Should().Be(OperationErrors.ProjectMaxNumberOfTasksAchieved(It.IsAny<int>()).Type);
    }

    [Fact]
    public async Task AddProjectTaskAsync_WhenTaskAddedSuccessfully_ReturnsSuccess()
    {
        // Arrange
        var userRepositoryMock = new Mock<IRepository<User>>();
        var projectRepositoryMock = new Mock<IRepository<Project>>();
        var taskRepositoryMock = new Mock<IRepository<ProjectTask>>();

        var user = GetUser();
        var assTaskDto = new AddTaskDto();

        var project = new Project();        

        userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(user);

        projectRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(project);

        var projectService = new ProjectService(
           projectRepositoryMock.Object,
           userRepositoryMock.Object,
           taskRepositoryMock.Object);

        // Act
        var result = await projectService.AddProjectTaskAsync(assTaskDto, It.IsAny<int>(), It.IsAny<int>());

        // Assert
        result.IsSuccess.Should().BeTrue();
    }


    [Fact]
    public async Task GetTasksAsync_ProjectNotFound_ReturnsFailure()
    {
        // Arrange
        var projectId = It.IsAny<int>();
        var userId = It.IsAny<int>();

        var projectRepositoryMock = new Mock<IRepository<Project>>();
        projectRepositoryMock.Setup(repo => repo.Exist(projectId)).ReturnsAsync(false);

        var userRepositoryMock = new Mock<IRepository<User>>();
        userRepositoryMock.Setup(repo => repo.Exist(userId)).ReturnsAsync(true);

        var taskRepositoryMock = new Mock<IRepository<ProjectTask>>();

        var projectService = new ProjectService(
            projectRepositoryMock.Object,
            userRepositoryMock.Object,
            taskRepositoryMock.Object);

        // Act
        var result = await projectService.GetTasksAsync(projectId, userId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        var error = result.Errors.FirstOrDefault();

        error.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        error.Type.Should().Be(OperationErrors.ProjectNotFound(It.IsAny<int>()).Type);

    }

    [Fact]
    public async Task GetTasksAsync_UserNotFound_ReturnsFailure()
    {
        // Arrange
        var projectId = It.IsAny<int>();
        var userId = It.IsAny<int>();

        var projectRepositoryMock = new Mock<IRepository<Project>>();
        projectRepositoryMock.Setup(repo => repo.Exist(projectId)).ReturnsAsync(true);

        var userRepositoryMock = new Mock<IRepository<User>>();
        userRepositoryMock.Setup(repo => repo.Exist(userId)).ReturnsAsync(false);

        var taskRepositoryMock = new Mock<IRepository<ProjectTask>>();

        var projectService = new ProjectService(
            projectRepositoryMock.Object,
            userRepositoryMock.Object,
            taskRepositoryMock.Object);

        // Act
        var result = await projectService.GetTasksAsync(projectId, userId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        var error = result.Errors.FirstOrDefault();

        error.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        error.Type.Should().Be(OperationErrors.UserNotFound(It.IsAny<int>()).Type);
    }    



    public User GetUser()
    {
        var faker = new Faker();
        return new User(faker.Name.FullName(), faker.Lorem.Sentence(1));
    }



}
