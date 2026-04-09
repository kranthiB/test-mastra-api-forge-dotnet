using ApiForge.Application.Common.Models;
using ApiForge.Application.GroupUserAssignments.DTOs;
using ApiForge.Application.GroupUserAssignments.Interfaces;
using ApiForge.Application.GroupUserAssignments.Services;
using ApiForge.Domain.GroupUserAssignments;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ApiForge.Tests.Application.GroupUserAssignments;

public class GroupUserAssignmentServiceTests
{
    private readonly Mock<IGroupUserAssignmentRepository> _repoMock = new();
    private readonly Mock<IValidator<CreateGroupUserAssignmentRequest>> _validatorMock = new();
    private readonly Mock<ILogger<GroupUserAssignmentService>> _loggerMock = new();
    private readonly GroupUserAssignmentService _sut;

    public GroupUserAssignmentServiceTests()
    {
        _sut = new GroupUserAssignmentService(_repoMock.Object, _validatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnSuccess_WhenRequestIsValidAndAssignmentDoesNotExist()
    {
        // Arrange
        var request = new CreateGroupUserAssignmentRequest(Guid.NewGuid(), Guid.NewGuid());
        _validatorMock.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new ValidationResult());
        _repoMock.Setup(r => r.ExistsByGroupAndUserAsync(request.GroupId, request.UserId, default)).ReturnsAsync(false);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.GroupId.Should().Be(request.GroupId);
        result.Value!.UserId.Should().Be(request.UserId);
        _repoMock.Verify(r => r.AddAsync(It.IsAny<GroupUserAssignment>(), default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnConflict_WhenAssignmentAlreadyExists()
    {
        // Arrange
        var request = new CreateGroupUserAssignmentRequest(Guid.NewGuid(), Guid.NewGuid());
        _validatorMock.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new ValidationResult());
        _repoMock.Setup(r => r.ExistsByGroupAndUserAsync(request.GroupId, request.UserId, default)).ReturnsAsync(true);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorKind.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnSuccess_WhenAssignmentExists()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();
        var assignment = GroupUserAssignment.Create(Guid.NewGuid(), Guid.NewGuid());
        _repoMock.Setup(r => r.GetByIdAsync(assignmentId, default)).ReturnsAsync(assignment);

        // Act
        var result = await _sut.DeleteAsync(assignmentId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _repoMock.Verify(r => r.DeleteAsync(assignment, default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnNotFound_WhenAssignmentDoesNotExist()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(assignmentId, default)).ReturnsAsync((GroupUserAssignment?)null);

        // Act
        var result = await _sut.DeleteAsync(assignmentId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorKind.Should().Be(ErrorType.NotFound);
    }
}
