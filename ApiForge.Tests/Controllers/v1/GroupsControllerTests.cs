
using ApiForge.Application.Common.Models;
using ApiForge.Application.Groups.DTOs;
using ApiForge.Application.Groups.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

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
}
