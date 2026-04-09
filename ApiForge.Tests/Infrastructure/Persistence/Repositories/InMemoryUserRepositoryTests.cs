
using ApiForge.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Xunit;

namespace ApiForge.Tests.Infrastructure.Persistence.Repositories;

public class InMemoryUserRepositoryTests
{
    private readonly InMemoryUserRepository _sut;

    public InMemoryUserRepositoryTests()
    {
        _sut = new InMemoryUserRepository();
    }

    [Fact]
    public async Task GetPaginatedAsync_ShouldReturnCorrectPage_WhenQueried()
    {
        // Arrange
        // The repository is seeded with 50 users in the constructor

        // Act
        var (items, total) = await _sut.GetPaginatedAsync(0, 10);

        // Assert
        total.Should().Be(50);
        items.Should().HaveCount(10);
    }

    [Fact]
    public async Task GetPaginatedAsync_ShouldReturnRemainingItems_WhenPageIsAtTheEnd()
    {
        // Arrange

        // Act
        var (items, total) = await _sut.GetPaginatedAsync(45, 10);

        // Assert
        total.Should().Be(50);
        items.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetPaginatedAsync_ShouldReturnEmpty_WhenOffsetIsOutOfBounds()
    {
        // Arrange

        // Act
        var (items, total) = await _sut.GetPaginatedAsync(100, 10);

        // Assert
        total.Should().Be(50);
        items.Should().BeEmpty();
    }
}
