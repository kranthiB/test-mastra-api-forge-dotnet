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

public sealed class GroupUserAssignmentsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly Mock<IGroupUserAssignmentService> _svcMock = new();
    private readonly HttpClient _client;

    public GroupUserAssignmentsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(b => b.ConfigureServices(s =>
            s.AddScoped<IGroupUserAssignmentService>(_ => _svcMock.Object))).CreateClient();
    }

    [Fact]
    public async Task Delete_Returns204_WhenFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _svcMock.Setup(x => x.DeleteAsync(id, default)).ReturnsAsync(Result<bool>.Success(true));

        // Act
        var response = await _client.DeleteAsync($"/api/v1/group-user-assignments/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_Returns404_WhenNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _svcMock.Setup(x => x.DeleteAsync(id, default)).ReturnsAsync(Result<bool>.NotFound("Not found"));

        // Act
        var response = await _client.DeleteAsync($"/api/v1/group-user-assignments/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_Returns201_WhenValid()
    {
        // Arrange
        var request = new CreateGroupUserAssignmentRequest(Guid.NewGuid(), Guid.NewGuid());
        var responseDto = new GroupUserAssignmentResponse(Guid.NewGuid(), request.GroupId, request.UserId, DateTime.UtcNow, null);
        _svcMock.Setup(x => x.CreateAsync(request, default)).ReturnsAsync(Result<GroupUserAssignmentResponse>.Success(responseDto));

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/group-user-assignments", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain(responseDto.Id.ToString());
    }

    [Fact]
    public async Task Create_Returns409_WhenConflict()
    {
        // Arrange
        var request = new CreateGroupUserAssignmentRequest(Guid.NewGuid(), Guid.NewGuid());
        _svcMock.Setup(x => x.CreateAsync(request, default)).ReturnsAsync(Result<GroupUserAssignmentResponse>.Conflict("Conflict"));

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/group-user-assignments", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Create_Returns400_WhenInvalid()
    {
        // Arrange
        var request = new CreateGroupUserAssignmentRequest(Guid.Empty, Guid.Empty);
        _svcMock.Setup(x => x.CreateAsync(request, default)).ReturnsAsync(Result<GroupUserAssignmentResponse>.Validation("Invalid"));

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/group-user-assignments", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
