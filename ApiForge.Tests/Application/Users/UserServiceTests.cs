
using ApiForge.Application.Users.DTOs;
using ApiForge.Application.Users.Interfaces;
using ApiForge.Application.Users.Services;
using ApiForge.Domain.Users;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ApiForge.Tests.Application.Users;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IGroupUserAssignmentRepository> _groupUserAssignmentRepositoryMock;
    private readonly Mock<IValidator<UpdateUserRequest>> _updateValidatorMock;
    private readonly Mock<ILogger<UserService>> _loggerMock;
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _groupUserAssignmentRepositoryMock = new Mock<IGroupUserAssignmentRepository>();
        _updateValidatorMock = new Mock<IValidator<UpdateUserRequest>>();
        _loggerMock = new Mock<ILogger<UserService>>();
        _sut = new UserService(_userRepositoryMock.Object, _groupUserAssignmentRepositoryMock.Object, _updateValidatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPaginatedUsers_WhenCalledWithValidParameters()
    {
        // Arrange
        var users = new List<User> { User.Create("test", "test@test.com") };
        _userRepositoryMock.Setup(r => r.GetPaginatedAsync(0, 25, default)).ReturnsAsync((users, 1));

        // Act
        var result = await _sut.GetAllAsync(0, 25);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Data.Should().HaveCount(1);
        result.Value.Total.Should().Be(1);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public async Task GetAllAsync_ShouldReturnValidationError_WhenLimitIsInvalid(int limit)
    {
        // Act
        var result = await _sut.GetAllAsync(0, limit);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorKind.Should().Be(ApiForge.Application.Common.Models.ErrorType.Validation);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, default)).ReturnsAsync((User)null);

        // Act
        var result = await _sut.DeleteAsync(userId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorKind.Should().Be(ApiForge.Application.Common.Models.ErrorType.NotFound);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnConflict_WhenUserHasGroupAssignments()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = User.Create("test", "test@test.com", "test");
        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, default)).ReturnsAsync(user);
        _groupUserAssignmentRepositoryMock.Setup(r => r.GetPaginatedAsync(0, 1, null, userId, default))
            .ReturnsAsync(new PagedResult<Domain.GroupUserAssignments.GroupUserAssignment>(new List<Domain.GroupUserAssignments.GroupUserAssignment> { GroupUserAssignment.Create("test-slug", Guid.NewGuid()) }, 1, 0, 1));

        // Act
        var result = await _sut.DeleteAsync(userId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorKind.Should().Be(ApiForge.Application.Common.Models.ErrorType.Conflict);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnSuccess_WhenUserIsDeleted()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = User.Create("test", "test@test.com", "test");
        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, default)).ReturnsAsync(user);
        _groupUserAssignmentRepositoryMock.Setup(r => r.GetPaginatedAsync(0, 1, null, userId, default))
            .ReturnsAsync(new PagedResult<Domain.GroupUserAssignments.GroupUserAssignment>(new List<Domain.GroupUserAssignments.GroupUserAssignment>(), 0, 0, 1));

        // Act
        var result = await _sut.DeleteAsync(userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _userRepositoryMock.Verify(r => r.DeleteAsync(user, default), Times.Once);
    }
}
}
