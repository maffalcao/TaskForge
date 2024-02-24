using Domain.Dtos;
using Domain.Entities;
using Domain.ErrorHandling;
using Domain.Interfaces.Persistence;
using Domain.Services;
using FluentAssertions;
using Moq;

public class ProjectServiceTests
{
    [Fact]
    public async Task AddAsync_WhenUserExists_ShouldReturnSuccess()
    {
        // Arrange
        var userId = 1;
        var projectDto = new AddProjectDto { Name = "Test Project" };

        var userRepositoryMock = new Mock<IRepository<User>>();
        userRepositoryMock.Setup(repo => repo.Exist(userId)).ReturnsAsync(true);

        var projectRepositoryMock = new Mock<IProjectRepository>();
        projectRepositoryMock.Setup(repo => repo.AddProjectAsync(It.IsAny<Project>()))
                             .ReturnsAsync(new Project(projectDto.Name, userId));

        var service = new ProjectService(projectRepositoryMock.Object, userRepositoryMock.Object);

        // Act
        var result = await service.AddAsync(projectDto, userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Result.Should().NotBeNull();
    }

    [Fact]
    public async Task AddAsync_WhenUserDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var userId = 500;
        var projectDto = new AddProjectDto { Name = "Test Project" };

        var userRepositoryMock = new Mock<IRepository<User>>();
        userRepositoryMock.Setup(repo => repo.Exist(userId)).ReturnsAsync(false);

        var service = new ProjectService(Mock.Of<IProjectRepository>(), userRepositoryMock.Object);

        // Act
        var result = await service.AddAsync(projectDto, userId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.First().Description.Should().Be(OperationErrors.UserNotFound(userId).Description);
    }
}
