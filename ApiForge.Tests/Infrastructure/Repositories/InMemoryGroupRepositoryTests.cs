using ApiForge.Domain.Groups;
using ApiForge.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Xunit;

namespace ApiForge.Tests.Infrastructure.Repositories;

public class InMemoryGroupRepositoryTests
{
    private readonly InMemoryGroupRepository _sut;
    private readonly Group _group1;
    private readonly Group _group2;

    public InMemoryGroupRepositoryTests()
    {
        _sut = new InMemoryGroupRepository();
        _group1 = Group.Create("Test Group 1", "Description 1");
        _group2 = Group.Create("Test Group 2", "Description 2");
        _sut.AddAsync(_group1).Wait();
        _sut.AddAsync(_group2).Wait();
    }

    [Fact]
    public async Task ExistsByNameAsync_ShouldReturnTrue_WhenGroupExists()
    {
        // Act
        var result = await _sut.ExistsByNameAsync("Test Group 1");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByNameAsync_ShouldReturnFalse_WhenGroupDoesNotExist()
    {
        // Act
        var result = await _sut.ExistsByNameAsync("Non-existent Group");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsByNameAsync_ShouldExcludeGivenId()
    {
        // Act
        var result = await _sut.ExistsByNameAsync("Test Group 1", _group1.Id);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsBySlugAsync_ShouldReturnTrue_WhenGroupExists()
    {
        // Act
        var result = await _sut.ExistsBySlugAsync("test-group-1");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsBySlugAsync_ShouldReturnFalse_WhenGroupDoesNotExist()
    {
        // Act
        var result = await _sut.ExistsBySlugAsync("non-existent-group");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsBySlugAsync_ShouldExcludeGivenId()
    {
        // Act
        var result = await _sut.ExistsBySlugAsync("test-group-1", _group1.Id);

        // Assert
        result.Should().BeFalse();
    }
}
