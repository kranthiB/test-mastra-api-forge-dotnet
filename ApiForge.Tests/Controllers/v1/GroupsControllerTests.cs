
using ApiForge.Application.Common.Models;
using ApiForge.Application.Groups.DTOs;
using ApiForge.Application.Groups.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ApiForge.Tests.Controllers.v1;

public sealed class GroupsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly Mock<IGroupService> _groupServiceMock = new();

    public GroupsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped(_ => _groupServiceMock.Object);
            });
        }).CreateClient();
    }

    [Fact]
    public async Task CreateGroup_Returns201Created_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateGroupRequest("Test Group", "test-group", "A test group.");
        var groupResponse = new GroupResponse(Guid.NewGuid(), request.GroupName, request.GroupSlug, request.GroupDesc, null, DateTime.UtcNow, null);
        _groupServiceMock.Setup(s => s.CreateAsync(request, default)).ReturnsAsync(Result<GroupResponse>.Success(groupResponse));

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/groups", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdGroup = await response.Content.ReadFromJsonAsync<GroupResponse>();
        createdGroup.Should().BeEquivalentTo(groupResponse);
        response.Headers.Location.Should().Be($"/api/v1/groups/{groupResponse.Id}");
    }

    [Fact]
    public async Task CreateGroup_Returns409Conflict_WhenGroupSlugAlreadyExists()
    {
        // Arrange
        var request = new CreateGroupRequest("Test Group", "test-group", "A test group.");
        _groupServiceMock.Setup(s => s.CreateAsync(request, default)).ReturnsAsync(Result<GroupResponse>.Conflict("A group with the same slug already exists."));

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/groups", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateGroup_Returns400BadRequest_WhenRequestIsInvalid()
    {
        // Arrange
        var request = new CreateGroupRequest("", "invalid slug", ""); // Invalid request
        _groupServiceMock.Setup(s => s.CreateAsync(request, default)).ReturnsAsync(Result<GroupResponse>.Validation("Validation failed"));

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/groups", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetGroups_Returns200OK_WithDefaultPagination()
    {
        // Arrange
        var groups = new List<GroupResponse> { new(Guid.NewGuid(), "Group 1", "group-1", "Desc 1", null, DateTime.UtcNow, null) };
        var pagedResult = new PagedResult<GroupResponse>(groups, 1, 0, 25);
        _groupServiceMock.Setup(s => s.GetPaginatedAsync(0, 25, default)).ReturnsAsync(Result<PagedResult<GroupResponse>>.Success(pagedResult));

        // Act
        var response = await _client.GetAsync("/api/v1/groups");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<GroupResponse>>();
        result.Should().BeEquivalentTo(pagedResult);
    }

    [Fact]
    public async Task GetGroups_Returns200OK_WithCustomPagination()
    {
        // Arrange
        var groups = new List<GroupResponse> { new(Guid.NewGuid(), "Group 1", "group-1", "Desc 1", null, DateTime.UtcNow, null) };
        var pagedResult = new PagedResult<GroupResponse>(groups, 1, 10, 50);
        _groupServiceMock.Setup(s => s.GetPaginatedAsync(10, 50, default)).ReturnsAsync(Result<PagedResult<GroupResponse>>.Success(pagedResult));

        // Act
        var response = await _client.GetAsync("/api/v1/groups?offset=10&limit=50");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<GroupResponse>>();
        result.Should().BeEquivalentTo(pagedResult);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public async Task GetGroups_Returns400BadRequest_WhenLimitIsInvalid(int limit)
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/groups?limit={limit}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetGroups_Returns200OK_WithEmptyData_WhenOffsetIsBeyondTotal()
    {
        // Arrange
        var pagedResult = new PagedResult<GroupResponse>(new List<GroupResponse>(), 20, 30, 10);
        _groupServiceMock.Setup(s => s.GetPaginatedAsync(30, 10, default)).ReturnsAsync(Result<PagedResult<GroupResponse>>.Success(pagedResult));

        // Act
        var response = await _client.GetAsync("/api/v1/groups?offset=30&limit=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<GroupResponse>>();
        result.Should().BeEquivalentTo(pagedResult);
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetGroup_Returns200OK_WhenGroupExists()
    {
        // Arrange
        var groupSlug = "test-group";
        var groupResponse = new GroupResponse(Guid.NewGuid(), "Test Group", groupSlug, "A test group.", null, DateTime.UtcNow, null);
        _groupServiceMock.Setup(s => s.GetBySlugAsync(groupSlug, default)).ReturnsAsync(Result<GroupResponse>.Success(groupResponse));

        // Act
        var response = await _client.GetAsync($"/api/v1/groups/{groupSlug}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetchedGroup = await response.Content.ReadFromJsonAsync<GroupResponse>();
        fetchedGroup.Should().BeEquivalentTo(groupResponse);
    }

    [Fact]
    public async Task GetGroup_Returns404NotFound_WhenGroupDoesNotExist()
    {
        // Arrange
        var groupSlug = "non-existent-group";
        _groupServiceMock.Setup(s => s.GetBySlugAsync(groupSlug, default)).ReturnsAsync(Result<GroupResponse>.NotFound("Group not found."));

        // Act
        var response = await _client.GetAsync($"/api/v1/groups/{groupSlug}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ReplaceGroup_Returns200OK_WhenRequestIsValid()
    {
        // Arrange
        var groupSlug = "test-group";
        var request = new UpdateGroupRequest("Updated Group Name", "Updated group description.");
        var updatedResponse = new GroupResponse(Guid.NewGuid(), request.GroupName, groupSlug, request.GroupDesc, null, DateTime.UtcNow, DateTime.UtcNow);
        _groupServiceMock.Setup(s => s.UpdateAsync(groupSlug, request, default)).ReturnsAsync(Result<GroupResponse>.Success(updatedResponse));

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/groups/{groupSlug}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var group = await response.Content.ReadFromJsonAsync<GroupResponse>();
        group.Should().BeEquivalentTo(updatedResponse);
    }

    [Fact]
    public async Task ReplaceGroup_Returns404NotFound_WhenGroupDoesNotExist()
    {
        // Arrange
        var groupSlug = "non-existent-group";
        var request = new UpdateGroupRequest("Updated Group Name", "Updated group description.");
        _groupServiceMock.Setup(s => s.UpdateAsync(groupSlug, request, default)).ReturnsAsync(Result<GroupResponse>.NotFound($"A group with slug '{groupSlug}' was not found."));

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/groups/{groupSlug}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ReplaceGroup_Returns409Conflict_WhenGroupNameIsTaken()
    {
        // Arrange
        var groupSlug = "test-group";
        var request = new UpdateGroupRequest("existing-group-name", "Updated group description.");
        _groupServiceMock.Setup(s => s.UpdateAsync(groupSlug, request, default)).ReturnsAsync(Result<GroupResponse>.Conflict("A group with the same name already exists."));

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/groups/{groupSlug}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ReplaceGroup_Returns400BadRequest_WhenRequestIsInvalid()
    {
        // Arrange
        var groupSlug = "test-group";
        var request = new UpdateGroupRequest("", ""); // Invalid
        _groupServiceMock.Setup(s => s.UpdateAsync(groupSlug, request, default)).ReturnsAsync(Result<GroupResponse>.Validation("Validation failed"));

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/groups/{groupSlug}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
