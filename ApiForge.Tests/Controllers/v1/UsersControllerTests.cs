
using System.Net;
using System.Net.Http.Json;
using ApiForge.Application.Common.Models;
using ApiForge.Application.Users.DTOs;
using ApiForge.Application.Users.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace ApiForge.Tests.Controllers.v1;

public sealed class UsersControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly Mock<IUserService> _svcMock = new();
    private readonly HttpClient _client;

    public UsersControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(b => b.ConfigureServices(s =>
            s.AddScoped(_ => _svcMock.Object))).CreateClient();
    }

    // Test data
    private static readonly Guid TestUserId = Guid.NewGuid();
    private static readonly UserResponse TestUserResponse = new(TestUserId, "testuser", "test@example.com", "test cname", DateTimeOffset.UtcNow, null);

    [Fact]
    public async Task Update_Returns200_WhenValid()
    {
        // Arrange
        var request = new UpdateUserRequest("updateduser", "updated@example.com");
        var updatedResponse = TestUserResponse with { UserName = request.UserName, Email = request.Email };
        _svcMock.Setup(x => x.UpdateAsync(TestUserId, request, default))
            .ReturnsAsync(Result<UserResponse>.Success(updatedResponse));

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/users/{TestUserId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<UserResponse>();
        content.Should().BeEquivalentTo(updatedResponse);
    }

    [Fact]
    public async Task Update_Returns404_WhenNotFound()
    {
        // Arrange
        var request = new UpdateUserRequest("any", "any@any.com");
        _svcMock.Setup(x => x.UpdateAsync(TestUserId, request, default))
            .ReturnsAsync(Result<UserResponse>.NotFound("User not found"));

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/users/{TestUserId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_Returns409_WhenConflict()
    {
        // Arrange
        var request = new UpdateUserRequest("any", "any@any.com");
        _svcMock.Setup(x => x.UpdateAsync(TestUserId, request, default))
            .ReturnsAsync(Result<UserResponse>.Conflict("Email already exists"));

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/users/{TestUserId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Update_Returns400_WhenInvalid()
    {
        // Arrange
        var request = new UpdateUserRequest("", ""); // Invalid data
        _svcMock.Setup(x => x.UpdateAsync(TestUserId, request, default))
            .ReturnsAsync(Result<UserResponse>.Validation("Invalid data"));

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/users/{TestUserId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_Returns204NoContent_WhenSuccessful()
    {
        // Arrange
        _svcMock.Setup(x => x.DeleteAsync(TestUserId, default))
            .ReturnsAsync(Result<bool>.Success(true));

        // Act
        var response = await _client.DeleteAsync($"/api/v1/users/{TestUserId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_Returns404NotFound_WhenUserDoesNotExist()
    {
        // Arrange
        _svcMock.Setup(x => x.DeleteAsync(TestUserId, default))
            .ReturnsAsync(Result<bool>.NotFound("User not found"));

        // Act
        var response = await _client.DeleteAsync($"/api/v1/users/{TestUserId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_Returns409Conflict_WhenUserHasGroupAssignments()
    {
        // Arrange
        _svcMock.Setup(x => x.DeleteAsync(TestUserId, default))
            .ReturnsAsync(Result<bool>.Conflict("User has active group assignments"));

        // Act
        var response = await _client.DeleteAsync($"/api/v1/users/{TestUserId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}
