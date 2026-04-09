
using ApiForge.Application.Users.Interfaces;
using ApiForge.Application.Users.Services;
using ApiForge.Domain.Users;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ApiForge.Tests.Application.Users;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ILogger<UserService>> _loggerMock;
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<UserService>>();
        _sut = new UserService(_userRepositoryMock.Object, _loggerMock.Object);
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
}
