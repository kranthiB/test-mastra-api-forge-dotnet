
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
    public async Task Create_Returns201_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateUserRequest("testuser", "test@example.com");
        var userResponse = new UserResponse(Guid.NewGuid(), request.UserName, request.Email, "testuser-cname", DateTime.UtcNow, null);
        _userServiceMock.Setup(s => s.CreateAsync(request, default)).ReturnsAsync(Result<UserResponse>.Success(userResponse));

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain(userResponse.Id.ToString());
        var createdUser = await response.Content.ReadFromJsonAsync<UserResponse>();
        createdUser.Should().BeEquivalentTo(userResponse);
    }

    [Fact]
    public async Task Create_Returns400_WhenRequestIsInvalid()
    {
        // Arrange
        var request = new CreateUserRequest("", ""); // Invalid request
        _userServiceMock.Setup(s => s.CreateAsync(request, default)).ReturnsAsync(Result<UserResponse>.Validation("Validation failed"));

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_Returns409_WhenEmailAlreadyExists()
    {
        // Arrange
        var request = new CreateUserRequest("testuser", "test@example.com");
        _userServiceMock.Setup(s => s.CreateAsync(request, default)).ReturnsAsync(Result<UserResponse>.Conflict("Email already exists"));

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}
