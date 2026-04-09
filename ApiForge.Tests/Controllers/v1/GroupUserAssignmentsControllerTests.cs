
using System.Net;
using System.Net.Http.Json;
using ApiForge.Application.Common.Models;
using ApiForge.Application.GroupUserAssignments.DTOs;
using ApiForge.Application.GroupUserAssignments.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace ApiForge.Tests.Controllers.v1;

public class GroupUserAssignmentsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly Mock<IGroupUserAssignmentService> _serviceMock = new();
    private readonly HttpClient _client;

    public GroupUserAssignmentsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(b => b.ConfigureServices(s =>
            s.AddScoped(_ => _serviceMock.Object))).CreateClient();
    }

    [Fact]
    public async Task GetAssignmentByIdAsync_Returns200_WhenFound()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();
        var assignmentResponse = new GroupUserAssignmentResponse(assignmentId, "test-slug", Guid.NewGuid(), DateTime.UtcNow, null);
        var result = Result<GroupUserAssignmentResponse>.Success(assignmentResponse);
        _serviceMock.Setup(s => s.GetByIdAsync(assignmentId, default)).ReturnsAsync(result);

        // Act
        var response = await _client.GetAsync($"/api/v1/group-user-assignments/{assignmentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<GroupUserAssignmentResponse>();
        content.Should().BeEquivalentTo(assignmentResponse);
    }

    [Fact]
    public async Task GetAssignmentByIdAsync_Returns404_WhenNotFound()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();
        var result = Result<GroupUserAssignmentResponse>.NotFound("Assignment not found.");
        _serviceMock.Setup(s => s.GetByIdAsync(assignmentId, default)).ReturnsAsync(result);

        // Act
        var response = await _client.GetAsync($"/api/v1/group-user-assignments/{assignmentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_Returns201_WhenValid()
    {
        // Arrange
        var request = new CreateGroupUserAssignmentRequest("test-group", Guid.NewGuid());
        var responseDto = new GroupUserAssignmentResponse(Guid.NewGuid(), request.GroupSlug, request.UserId, DateTime.UtcNow, null);
        var result = Result<GroupUserAssignmentResponse>.Success(responseDto);
        _serviceMock.Setup(s => s.CreateAsync(request, default)).ReturnsAsync(result);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/group-user-assignments", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Be($"/api/v1/group-user-assignments/{responseDto.GroupUserAssignId}");
    }

    [Fact]
    public async Task GetAll_Returns200_WithDefaultPagination()
    {
        // Arrange
        var pagedResult = new PagedResult<GroupUserAssignmentResponse>(new List<GroupUserAssignmentResponse>(), 0, 0, 25);
        var result = Result<PagedResult<GroupUserAssignmentResponse>>.Success(pagedResult);
        _serviceMock.Setup(s => s.GetPaginatedAsync(0, 25, null, null, default)).ReturnsAsync(result);

        // Act
        var response = await _client.GetAsync("/api/v1/group-user-assignments");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<PagedResult<GroupUserAssignmentResponse>>();
        content.Should().NotBeNull();
        content.Limit.Should().Be(25);
        content.Offset.Should().Be(0);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public async Task GetAll_Returns400_ForInvalidLimit(int limit)
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/group-user-assignments?limit={limit}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAll_Returns400_ForInvalidOffset()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/group-user-assignments?offset=-1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAll_Returns200_WhenFilteringByGroupSlug()
    {
        // Arrange
        var groupSlug = "test-group";
        var pagedResult = new PagedResult<GroupUserAssignmentResponse>(new List<GroupUserAssignmentResponse>(), 0, 0, 25);
        var result = Result<PagedResult<GroupUserAssignmentResponse>>.Success(pagedResult);
        _serviceMock.Setup(s => s.GetPaginatedAsync(0, 25, groupSlug, null, default)).ReturnsAsync(result);

        // Act
        var response = await _client.GetAsync($"/api/v1/group-user-assignments?groupSlug={groupSlug}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _serviceMock.Verify(s => s.GetPaginatedAsync(0, 25, groupSlug, null, default), Times.Once);
    }

    [Fact]
    public async Task GetAll_Returns200_WhenFilteringByUserId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var pagedResult = new PagedResult<GroupUserAssignmentResponse>(new List<GroupUserAssignmentResponse>(), 0, 0, 25);
        var result = Result<PagedResult<GroupUserAssignmentResponse>>.Success(pagedResult);
        _serviceMock.Setup(s => s.GetPaginatedAsync(0, 25, null, userId, default)).ReturnsAsync(result);

        // Act
        var response = await _client.GetAsync($"/api/v1/group-user-assignments?userId={userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _serviceMock.Verify(s => s.GetPaginatedAsync(0, 25, null, userId, default), Times.Once);
    }

    [Fact]
    public async Task DeleteAssignmentAsync_Returns204_WhenSuccessful()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();
        var result = Result<bool>.Success(true);
        _serviceMock.Setup(s => s.DeleteAsync(assignmentId, default)).ReturnsAsync(result);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/group-user-assignments/{assignmentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteAssignmentAsync_Returns404_WhenNotFound()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();
        var result = Result<bool>.NotFound("Assignment not found.");
        _serviceMock.Setup(s => s.DeleteAsync(assignmentId, default)).ReturnsAsync(result);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/group-user-assignments/{assignmentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
