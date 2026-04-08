
using ApiForge.Application.Users.Interfaces;
using ApiForge.Application.Users.Services;
using ApiForge.Domain.Users;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace ApiForge.Tests.Application.Users;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _repoMock;
    private readonly Mock<ILogger<UserService>> _loggerMock;
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _repoMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<UserService>>();
        _sut = new UserService(_repoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPagedResult_WhenUsersExist()
    {
        // Arrange
        var users = new List<User>
        {
            User.Create("user1", "user1@test.com", null),
            User.Create("user2", "user2@test.com", null)
        };
        _repoMock.Setup(r => r.GetPagedAsync(0, 10, default)).ReturnsAsync((users, 2));

        // Act
        var result = await _sut.GetAllAsync(0, 10, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Items.Should().HaveCount(2);
        result.Value.Total.Should().Be(2);
        result.Value.Offset.Should().Be(0);
        result.Value.Limit.Should().Be(10);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyPagedResult_WhenNoUsersExist()
    {
        // Arrange
        _repoMock.Setup(r => r.GetPagedAsync(0, 10, default)).ReturnsAsync((new List<User>(), 0));

        // Act
        var result = await _sut.GetAllAsync(0, 10, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Items.Should().BeEmpty();
        result.Value.Total.Should().Be(0);
    }
}
