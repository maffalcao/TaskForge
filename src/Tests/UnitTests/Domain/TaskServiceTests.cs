using Domain.Dtos;
using Domain.Entities;
using Domain.ErrorHandling;
using Domain.Interfaces.Persistence;
using Domain.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Tests.UnitTests.Domain;
public class TaskServiceTests
{
    [Fact]
    public async Task UpdateTask_PriorityUnchangeable_Fails()
    {
        // Arrange
        var projectRepository = new Mock<IRepository<Project>>();
        var userRepository = new Mock<IRepository<User>>();
        var taskRepository = new Mock<IRepository<ProjectTask>>();

        var userId = 1;
        var project = new Project();
        var user = new User("Marco", "manager");

        var task = new ProjectTask { Id = 1, Priority = ProjectTaskPriority.Low };
        var updateTaskDto = new UpdateTaskDto { Priority = ProjectTaskPriority.High };

        taskRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(task);
        projectRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(project);
        userRepository.Setup(repo => repo.Exist(userId)).ReturnsAsync(true);

        var taskService = new TaskService(projectRepository.Object, userRepository.Object, taskRepository.Object);

        // Act
        var result = await taskService.UpdateAsync(updateTaskDto, task.Id, userId);

        // Assert
        result.IsSuccess.Should().Be(false);

        var error = result.Errors.FirstOrDefault();

        error.StatusCode.Should().Be(StatusCodes.Status409Conflict);
        error.Type.Should().Be(OperationErrors.TaskPriorityCantBechanged(It.IsAny<int>()).Type);
    }
}
