
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
    private readonly Mock<IUserService> _userServiceMock = new();
    private readonly HttpClient _client;

    public UsersControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped<IUserService>(_ => _userServiceMock.Object);
            });
        }).CreateClient();
    }

    [Fact]
    public async Task GetUsers_Returns200_WithPagedData()
    {
        // Arrange
        var users = new List<UserResponse>
        {
            new(Guid.NewGuid(), "testuser1", "test1@email.com", "Test User 1", "test cname", DateTime.UtcNow, null),
            new(Guid.NewGuid(), "testuser2", "test2@email.com", "Test User 2", "test cname 2", DateTime.UtcNow, null)
        };
        var links = new { self = new { href = "/users?offset=0&limit=2" } };
        var pagedResult = new OffsetPagedResult<UserResponse>(users, 10, 0, 2, links);

        _userServiceMock.Setup(s => s.GetUsersAsync(0, 2, default))
            .ReturnsAsync(Result<OffsetPagedResult<UserResponse>>.Success(pagedResult));

        // Act
        var response = await _client.GetAsync("/api/v1/users?offset=0&limit=2");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<OffsetPagedResult<UserResponse>>();
        content.Should().NotBeNull();
        content!.Items.Should().HaveCount(2);
        content.Total.Should().Be(10);
    }

    [Fact]
    public async Task GetById_Returns200_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userResponse = new UserResponse(userId, "testuser", "test@email.com", "Test User", "test-cname", DateTime.UtcNow, null);
        _userServiceMock.Setup(s => s.GetByIdAsync(userId, default))
            .ReturnsAsync(Result<UserResponse>.Success(userResponse));

        // Act
        var response = await _client.GetAsync($"/api/v1/users/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<UserResponse>();
        content.Should().NotBeNull();
        content!.UserId.Should().Be(userId);
        content.UserName.Should().Be("testuser");
    }

    [Fact]
    public async Task GetById_Returns404_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userServiceMock.Setup(s => s.GetByIdAsync(userId, default))
            .ReturnsAsync(Result<UserResponse>.NotFound($"User with ID {userId} not found."));

        // Act
        var response = await _client.GetAsync($"/api/v1/users/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_Returns201_WhenValid()
    {
        // Arrange
        var request = new CreateUserRequest("newUser", "new@test.com", "New User", "new-cname");
        var responseDto = new UserResponse(Guid.NewGuid(), request.UserName, request.Email, request.DisplayName, request.CName, DateTime.UtcNow, null);

        _userServiceMock.Setup(s => s.CreateAsync(request, default))
            .ReturnsAsync(Result<UserResponse>.Success(responseDto));

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Be($"/api/v1/users/{responseDto.UserId}");

        var content = await response.Content.ReadFromJsonAsync<UserResponse>();
        content.Should().BeEquivalentTo(responseDto);
    }

    [Fact]
    public async Task Create_Returns409_WhenConflict()
    {
        // Arrange
        var request = new CreateUserRequest("existingUser", "existing@test.com", "Existing User", "existing-cname");
        _userServiceMock.Setup(s => s.CreateAsync(request, default))
            .ReturnsAsync(Result<UserResponse>.Conflict("User with the same username already exists."));

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Create_Returns400_WhenInvalid()
    {
        // Arrange
        var request = new CreateUserRequest("", "", "", ""); // Invalid request
        _userServiceMock.Setup(s => s.CreateAsync(request, default))
            .ReturnsAsync(Result<UserResponse>.Validation("Username is required."));

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
