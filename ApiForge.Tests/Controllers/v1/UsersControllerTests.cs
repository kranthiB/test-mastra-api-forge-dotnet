
using ApiForge.Application.Common.Models;
using ApiForge.Application.Users.DTOs;
using ApiForge.Application.Users.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace ApiForge.Tests.Controllers.v1;

public sealed class UsersControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly Mock<IUserService> _userServiceMock = new();
    private readonly HttpClient _client;

    public UsersControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped(_ => _userServiceMock.Object);
            });
        }).CreateClient();
    }

    [Fact]
    public async Task GetById_Returns200_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userResponse = new UserResponse { UserId = userId, UserName = "testuser", Email = "test@test.com", Cname = "test cname" };
        _userServiceMock.Setup(s => s.GetByIdAsync(userId, default))
            .ReturnsAsync(Result<UserResponse>.Success(userResponse));

        // Act
        var response = await _client.GetAsync($"/api/v1/users/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<UserResponse>();
        content.Should().NotBeNull();
        content.Should().BeEquivalentTo(userResponse);
    }

    [Fact]
    public async Task GetById_Returns404_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userServiceMock.Setup(s => s.GetByIdAsync(userId, default))
            .ReturnsAsync(Result<UserResponse>.NotFound("User not found"));

        // Act
        var response = await _client.GetAsync($"/api/v1/users/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAll_Returns200_WithDefaultPagination()
    {
        // Arrange
        var users = new List<UserResponse> { new UserResponse { UserId = Guid.NewGuid(), UserName = "test" } };
        var paginatedResponse = new PaginatedResponseDto<UserResponse>(0, 25, 1, new Dictionary<string, object>(), users);
        _userServiceMock.Setup(s => s.GetAllAsync(0, 25, default)).ReturnsAsync(Result<PaginatedResponseDto<UserResponse>>.Success(paginatedResponse));

        // Act
        var response = await _client.GetAsync("/api/v1/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<PaginatedResponseDto<UserResponse>>();
        content.Should().NotBeNull();
        content!.Data.Should().HaveCount(1);
        content.Offset.Should().Be(0);
        content.Limit.Should().Be(25);
    }

    [Fact]
    public async Task GetAll_Returns200_WithCustomPagination()
    {
        // Arrange
        var users = new List<UserResponse> { new UserResponse { UserId = Guid.NewGuid(), UserName = "test" } };
        var paginatedResponse = new PaginatedResponseDto<UserResponse>(5, 10, 1, new Dictionary<string, object>(), users);
        _userServiceMock.Setup(s => s.GetAllAsync(5, 10, default)).ReturnsAsync(Result<PaginatedResponseDto<UserResponse>>.Success(paginatedResponse));

        // Act
        var response = await _client.GetAsync("/api/v1/users?offset=5&limit=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<PaginatedResponseDto<UserResponse>>();
        content.Should().NotBeNull();
        content!.Offset.Should().Be(5);
        content.Limit.Should().Be(10);
    }

    [Fact]
    public async Task GetAll_Returns400_WhenLimitIsInvalid()
    {
        // Arrange
        _userServiceMock.Setup(s => s.GetAllAsync(0, 200, default)).ReturnsAsync(Result<PaginatedResponseDto<UserResponse>>.Validation("Limit must be between 1 and 100."));

        // Act
        var response = await _client.GetAsync("/api/v1/users?limit=200");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAll_Returns200_AndEmptyList_WhenOffsetIsTooHigh()
    {
        // Arrange
        var users = new List<UserResponse>();
        var paginatedResponse = new PaginatedResponseDto<UserResponse>(100, 25, 50, new Dictionary<string, object>(), users);
        _userServiceMock.Setup(s => s.GetAllAsync(100, 25, default)).ReturnsAsync(Result<PaginatedResponseDto<UserResponse>>.Success(paginatedResponse));

        // Act
        var response = await _client.GetAsync("/api/v1/users?offset=100");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<PaginatedResponseDto<UserResponse>>();
        content.Should().NotBeNull();
        content!.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task Update_Returns200_WhenRequestIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new UpdateUserRequest("updateduser", "updated@test.com");
        var userResponse = new UserResponse { UserId = userId, UserName = request.UserName, Email = request.Email };
        _userServiceMock.Setup(s => s.UpdateAsync(userId, request, default))
            .ReturnsAsync(Result<UserResponse>.Success(userResponse));

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/users/{userId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<UserResponse>();
        content.Should().BeEquivalentTo(userResponse);
    }

    [Fact]
    public async Task Update_Returns404_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new UpdateUserRequest("updateduser", "updated@test.com");
        _userServiceMock.Setup(s => s.UpdateAsync(userId, request, default))
            .ReturnsAsync(Result<UserResponse>.NotFound("User not found"));

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/users/{userId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_Returns409_WhenEmailIsInUse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new UpdateUserRequest("updateduser", "existing@test.com");
        _userServiceMock.Setup(s => s.UpdateAsync(userId, request, default))
            .ReturnsAsync(Result<UserResponse>.Conflict("Email already in use"));

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/users/{userId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Update_Returns400_WhenRequestIsInvalid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new UpdateUserRequest("", "invalid-email"); // Invalid request
        _userServiceMock.Setup(s => s.UpdateAsync(userId, request, default))
            .ReturnsAsync(Result<UserResponse>.Validation("Invalid request"));

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/users/{userId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
