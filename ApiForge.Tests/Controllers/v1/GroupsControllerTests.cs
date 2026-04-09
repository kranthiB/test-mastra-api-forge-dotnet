
using System.Net;
using System.Net.Http.Json;
using ApiForge.Application.Common.Models;
using ApiForge.Application.Groups.DTOs;
using ApiForge.Application.Groups.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ApiForge.Tests.Controllers.v1;

public sealed class GroupsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly Mock<IGroupService> _svcMock = new();
    private readonly HttpClient _client;

    public GroupsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(b => b.ConfigureServices(s =>
            s.AddScoped(_ => _svcMock.Object))).CreateClient();
    }

    [Fact]
    public async Task Create_Returns201_WhenValid()
    {
        // Arrange
        var request = new CreateGroupRequest("Test Group", "test-group", "A test group.");
        var responseDto = new GroupResponse(Guid.NewGuid(), request.GroupName, request.GroupSlug, request.GroupDesc, DateTime.UtcNow, null);
        _svcMock.Setup(x => x.CreateAsync(It.IsAny<CreateGroupRequest>(), default))
            .ReturnsAsync(Result<GroupResponse>.Success(responseDto));

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/groups", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdGroup = await response.Content.ReadFromJsonAsync<GroupResponse>();
        createdGroup.Should().NotBeNull();
        response.Headers.Location.Should().Be($"/api/v1/groups/{createdGroup!.Id}?version=1.0");
    }

    [Fact]
    public async Task Create_Returns409_WhenConflict()
    {
        // Arrange
        var request = new CreateGroupRequest("Test Group", "test-group", "A test group.");
        _svcMock.Setup(x => x.CreateAsync(It.IsAny<CreateGroupRequest>(), default))
            .ReturnsAsync(Result<GroupResponse>.Conflict("already exists"));

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/groups", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Create_Returns400_WhenInvalid()
    {
        // Arrange
        var request = new CreateGroupRequest("Test Group", "invalid slug", "A test group.");
        _svcMock.Setup(x => x.CreateAsync(It.IsAny<CreateGroupRequest>(), default))
            .ReturnsAsync(Result<GroupResponse>.Validation("invalid slug"));

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/groups", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAll_Returns200_WithDefaultParameters()
    {
        // Arrange
        var groups = new List<GroupResponse>
        {
            new(Guid.NewGuid(), "Group 1", "group-1", "Description 1", DateTimeOffset.UtcNow, null),
            new(Guid.NewGuid(), "Group 2", "group-2", "Description 2", DateTimeOffset.UtcNow, null)
        };
        var pagedResult = new OffsetPagedResult<GroupResponse>(groups, 2, 0, 25);
        _svcMock.Setup(s => s.GetAllAsync(0, 25, default))
            .ReturnsAsync(Result<OffsetPagedResult<GroupResponse>>.Success(pagedResult));

        // Act
        var response = await _client.GetAsync("/api/v1/groups");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<OffsetPagedResult<GroupResponse>>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        content.Should().NotBeNull();
        content!.Data.Should().HaveCount(2);
        content.Total.Should().Be(2);
        content.Offset.Should().Be(0);
        content.Limit.Should().Be(25);
    }

    [Fact]
    public async Task GetAll_Returns200_WithCustomParameters()
    {
        // Arrange
        var groups = new List<GroupResponse>(); // Empty list for offset beyond items
        var pagedResult = new OffsetPagedResult<GroupResponse>(groups, 5, 10, 5);
        _svcMock.Setup(s => s.GetAllAsync(10, 5, default))
            .ReturnsAsync(Result<OffsetPagedResult<GroupResponse>>.Success(pagedResult));

        // Act
        var response = await _client.GetAsync("/api/v1/groups?offset=10&limit=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<OffsetPagedResult<GroupResponse>>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        content.Should().NotBeNull();
        content!.Data.Should().BeEmpty();
        content.Total.Should().Be(5);
        content.Offset.Should().Be(10);
        content.Limit.Should().Be(5);
    }

    [Theory]
    [InlineData("limit=0")]
    [InlineData("limit=1001")]
    [InlineData("offset=-1")]
    public async Task GetAll_Returns400_ForInvalidParameters(string query)
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/groups?{query}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetBySlug_Returns200_WhenFound()
    {
        // Arrange
        var groupSlug = "test-group";
        var responseDto = new GroupResponse(Guid.NewGuid(), "Test Group", groupSlug, "A test group.", DateTime.UtcNow, null);
        _svcMock.Setup(x => x.GetBySlugAsync(groupSlug, default))
            .ReturnsAsync(Result<GroupResponse>.Success(responseDto));

        // Act
        var response = await _client.GetAsync($"/api/v1/groups/{groupSlug}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var group = await response.Content.ReadFromJsonAsync<GroupResponse>();
        group.Should().NotBeNull();
        group.Should().BeEquivalentTo(responseDto);
    }

    [Fact]
    public async Task GetBySlug_Returns404_WhenNotFound()
    {
        // Arrange
        var groupSlug = "non-existent-group";
        _svcMock.Setup(x => x.GetBySlugAsync(groupSlug, default))
            .ReturnsAsync(Result<GroupResponse>.NotFound("not found"));

        // Act
        var response = await _client.GetAsync($"/api/v1/groups/{groupSlug}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
