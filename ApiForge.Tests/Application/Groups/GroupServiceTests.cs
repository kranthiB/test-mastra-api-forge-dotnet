
using ApiForge.Application.Common.Models;
using ApiForge.Application.Groups.DTOs;
using ApiForge.Application.Groups.Interfaces;
using ApiForge.Application.Groups.Services;
using ApiForge.Application.Groups.Validators;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ApiForge.Tests.Application.Groups;

public class GroupServiceTests
{
    private readonly Mock<IGroupRepository> _repoMock;
    private readonly IValidator<CreateGroupRequest> _createValidator;
    private readonly IValidator<UpdateGroupRequest> _updateValidator;
    private readonly Mock<ILogger<GroupService>> _loggerMock;
    private readonly GroupService _sut;

    public GroupServiceTests()
    {
        _repoMock = new Mock<IGroupRepository>();
        _createValidator = new CreateGroupRequestValidator(); // Using real validator for this test
        _updateValidator = new UpdateGroupRequestValidator(); // Using real validator for this test
        _loggerMock = new Mock<ILogger<GroupService>>();
        _sut = new GroupService(_repoMock.Object, _createValidator, _updateValidator, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnSuccess_WhenRequestIsValidAndNameIsUnique()
    {
        // Arrange
        var request = new CreateGroupRequest("Test Group", "Test Description");
        _repoMock.Setup(r => r.ExistsByNameAsync(request.Name, null, default)).ReturnsAsync(false);
        _repoMock.Setup(r => r.ExistsBySlugAsync(It.IsAny<string>(), null, default)).ReturnsAsync(false);

        // Act
        var result = await _sut.CreateAsync(request, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Name.Should().Be(request.Name);
        result.Value.GroupSlug.Should().Be("test-group");
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Domain.Groups.Group>(), default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnValidationError_WhenNameIsEmpty()
    {
        // Arrange
        var request = new CreateGroupRequest("", "Test Description");

        // Act
        var result = await _sut.CreateAsync(request, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorKind.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnConflictError_WhenNameIsNotUnique()
    {
        // Arrange
        var request = new CreateGroupRequest("Test Group", "Test Description");
        _repoMock.Setup(r => r.ExistsByNameAsync(request.Name, null, default)).ReturnsAsync(true);

        // Act
        var result = await _sut.CreateAsync(request, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorKind.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnConflictError_WhenSlugIsNotUnique()
    {
        // Arrange
        var request = new CreateGroupRequest("Test Group", "Test Description");
        _repoMock.Setup(r => r.ExistsByNameAsync(request.Name, null, default)).ReturnsAsync(false);
        _repoMock.Setup(r => r.ExistsBySlugAsync("test-group", null, default)).ReturnsAsync(true);

        // Act
        var result = await _sut.CreateAsync(request, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorKind.Should().Be(ErrorType.Conflict);
    }
}
