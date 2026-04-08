using System.Net;
using System.Net.Http.Json;
using ApiForge.Application.Common.Models;
using ApiForge.Application.Groups.DTOs;
using ApiForge.Application.Groups.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
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
    public async Task Create_Returns201_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateGroupRequest("New Group", "A cool new group.");
        var responseDto = new GroupResponse(Guid.NewGuid(), "new-group", request.Name, request.Description, DateTimeOffset.UtcNow, null);
        _svcMock.Setup(x => x.CreateAsync(request, default))
            .ReturnsAsync(Result<GroupResponse>.Success(responseDto));

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/groups", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<GroupResponse>();
        content.Should().BeEquivalentTo(responseDto);
        response.Headers.Location.Should().Be($"/api/v1/groups/{content!.Id}");
    }

    [Fact]
    public async Task Create_Returns400_WhenRequestIsInvalid()
    {
        // Arrange
        var request = new CreateGroupRequest("", null); // Invalid name
        _svcMock.Setup(x => x.CreateAsync(request, default))
            .ReturnsAsync(Result<GroupResponse>.Validation("Name is required."));

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/groups", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_Returns409_WhenGroupAlreadyExists()
    {
        // Arrange
        var request = new CreateGroupRequest("Existing Group", null);
        _svcMock.Setup(x => x.CreateAsync(request, default))
            .ReturnsAsync(Result<GroupResponse>.Conflict("Group with that name already exists."));

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/groups", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetAll_Returns200_WithPagedGroups()
    {
        // Arrange
        var groups = new List<GroupResponse>
        {
            new(Guid.NewGuid(), "admins", "Admins", null, DateTimeOffset.UtcNow, null),
            new(Guid.NewGuid(), "users", "Users", null, DateTimeOffset.UtcNow, null)
        };
        var pagedResult = new PagedResult<GroupResponse>(groups, groups.Count, 1, 10);
        var successResult = Result<PagedResult<GroupResponse>>.Success(pagedResult);

        _svcMock.Setup(x => x.GetAllAsync(1, 10, default)).ReturnsAsync(successResult);

        // Act
        var response = await _client.GetAsync("/api/v1/groups?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<PagedResult<GroupResponse>>();
        content.Should().NotBeNull();
        content!.Items.Should().HaveCount(2);
        content.TotalCount.Should().Be(2);
    }
}
