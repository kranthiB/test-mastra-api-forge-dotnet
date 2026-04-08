
using ApiForge.Domain.Users;
using ApiForge.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Xunit;

namespace ApiForge.Tests.Infrastructure.Repositories;

public class InMemoryUserRepositoryTests
{
    private readonly InMemoryUserRepository _sut;

    public InMemoryUserRepositoryTests()
    {
        _sut = new InMemoryUserRepository();
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnCorrectPage_WhenUsersExist()
    {
        // Arrange
        // The repository is seeded with 3 users

        // Act
        var (items, total) = await _sut.GetPagedAsync(0, 2, default);

        // Assert
        total.Should().Be(3);
        items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnRemainingItems_WhenLimitExceedsAvailable()
    {
        // Arrange

        // Act
        var (items, total) = await _sut.GetPagedAsync(1, 2, default);

        // Assert
        total.Should().Be(3);
        items.Should().HaveCount(2);
        // The default sorting is by CreatedAt descending.
        // The seed data is created sequentially, so the last one created is first.
        items.First().UserName.Should().Be("Bob");
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnEmpty_WhenOffsetIsPastTotal()
    {
        // Arrange

        // Act
        var (items, total) = await _sut.GetPagedAsync(3, 2, default);

        // Assert
        total.Should().Be(3);
        items.Should().BeEmpty();
    }

    [Fact]
    public async Task ExistsByEmailAsync_ShouldReturnTrue_WhenEmailExists()
    {
        // Arrange

        // Act
        var exists = await _sut.ExistsByEmailAsync("alice@example.com", null, default);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByEmailAsync_ShouldReturnFalse_WhenEmailDoesNotExist()
    {
        // Arrange

        // Act
        var exists = await _sut.ExistsByEmailAsync("nouser@example.com", null, default);

        // Assert
        exists.Should().BeFalse();
    }
}
