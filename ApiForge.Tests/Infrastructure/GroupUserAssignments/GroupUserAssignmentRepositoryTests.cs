using ApiForge.Domain.GroupUserAssignments;
using ApiForge.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Xunit;

namespace ApiForge.Tests.Infrastructure.GroupUserAssignments;

public class GroupUserAssignmentRepositoryTests
{
    private readonly InMemoryGroupUserAssignmentRepository _sut = new();

    [Fact]
    public async Task AddAsync_ShouldAddAssignment()
    {
        // Arrange
        var assignment = GroupUserAssignment.Create(Guid.NewGuid(), Guid.NewGuid());

        // Act
        await _sut.AddAsync(assignment);
        var result = await _sut.GetByIdAsync(assignment.Id);

        // Assert
        result.Should().Be(assignment);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveAssignment()
    {
        // Arrange
        var assignment = GroupUserAssignment.Create(Guid.NewGuid(), Guid.NewGuid());
        await _sut.AddAsync(assignment);

        // Act
        await _sut.DeleteAsync(assignment);
        var result = await _sut.GetByIdAsync(assignment.Id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ExistsByGroupAndUserAsync_ShouldReturnTrue_WhenAssignmentExists()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var assignment = GroupUserAssignment.Create(groupId, userId);
        await _sut.AddAsync(assignment);

        // Act
        var result = await _sut.ExistsByGroupAndUserAsync(groupId, userId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByGroupAndUserAsync_ShouldReturnFalse_WhenAssignmentDoesNotExist()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var result = await _sut.ExistsByGroupAndUserAsync(groupId, userId);

        // Assert
        result.Should().BeFalse();
    }
}
