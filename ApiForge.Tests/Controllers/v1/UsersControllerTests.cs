
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
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
        {
            var serviceDescriptor = s.SingleOrDefault(d => d.ServiceType == typeof(IUserService));
            if (serviceDescriptor != null)
            {
                s.Remove(serviceDescriptor);
            }
            s.AddScoped<IUserService>(_ => _svcMock.Object);
        })).CreateClient();
    }

    // POST /api/v1/users

    [Fact]
    public async Task Create_Returns201_WhenValid()
    {
        // Arrange
        var request = new CreateUserRequest("testuser", "test@example.com");
        var responseDto = new UserResponse(Guid.NewGuid(), request.UserName, request.Email, null, DateTime.UtcNow, null);

        _svcMock.Setup(x => x.CreateAsync(It.IsAny<CreateUserRequest>(), default))
                .ReturnsAsync(Result<UserResponse>.Success(responseDto));

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().EndWith($"/api/v1/users/{responseDto.Id}");

        var createdUser = await response.Content.ReadFromJsonAsync<UserResponse>();
        createdUser.Should().BeEquivalentTo(responseDto);
    }

    [Fact]
    public async Task Create_Returns409_WhenConflict()
    {
        // Arrange
        var request = new CreateUserRequest("existinguser", "existing@example.com");

        _svcMock.Setup(x => x.CreateAsync(It.IsAny<CreateUserRequest>(), default))
                .ReturnsAsync(Result<UserResponse>.Conflict("already exists"));

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Theory]
    [InlineData("", "test@test.com")] // Missing username
    [InlineData("testuser", "")] // Missing email
    public async Task Create_Returns400_WhenInvalid(string userName, string email)
    {
        // Arrange
        var request = new CreateUserRequest(userName, email);

        _svcMock.Setup(x => x.CreateAsync(It.IsAny<CreateUserRequest>(), default))
                .ReturnsAsync(Result<UserResponse>.Validation("Invalid data"));

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // GET /api/v1/users

    [Fact]
    public async Task GetAll_Returns200_WithPagedUsers()
    {
        // Arrange
        var users = new List<UserResponse>
        {
            new(Guid.NewGuid(), "user1", "user1@test.com", null, DateTime.UtcNow, null),
            new(Guid.NewGuid(), "user2", "user2@test.com", null, DateTime.UtcNow, null)
        };
        var pagedResult = new OffsetPagedResult<UserResponse>(users, 2, 0, 25);
        var result = Result<OffsetPagedResult<UserResponse>>.Success(pagedResult);

        _svcMock.Setup(s => s.GetAllAsync(0, 25, default)).ReturnsAsync(result);

        // Act
        var response = await _client.GetAsync("/api/v1/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<OffsetPagedResult<UserResponse>>();
        content.Should().NotBeNull();
        content!.Items.Should().HaveCount(2);
        content.Total.Should().Be(2);
    }

    // GET /api/v1/users/{id}

    [Fact]
    public async Task GetById_Returns200_WhenFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userDto = new UserResponse(userId, "testuser", "test@example.com", "test cname", DateTime.UtcNow, null);
        _svcMock.Setup(x => x.GetByIdAsync(userId, default))
            .ReturnsAsync(Result<UserResponse>.Success(userDto));

        // Act
        var response = await _client.GetAsync($"/api/v1/users/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetchedUser = await response.Content.ReadFromJsonAsync<UserResponse>();
        fetchedUser.Should().BeEquivalentTo(userDto);
    }

    [Fact]
    public async Task GetById_Returns404_WhenNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _svcMock.Setup(x => x.GetByIdAsync(userId, default))
            .ReturnsAsync(Result<UserResponse>.NotFound("User not found."));

        // Act
        var response = await _client.GetAsync($"/api/v1/users/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // PUT /api/v1/users/{id}

    [Fact]
    public async Task Update_Returns200_WhenValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new ReplaceUserRequest("updateduser", "updated@example.com");
        var responseDto = new UserResponse(userId, request.UserName, request.Email, null, DateTime.UtcNow, DateTime.UtcNow);

        _svcMock.Setup(x => x.UpdateAsync(userId, request, default))
            .ReturnsAsync(Result<UserResponse>.Success(responseDto));

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/users/{userId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedUser = await response.Content.ReadFromJsonAsync<UserResponse>();
        updatedUser.Should().BeEquivalentTo(responseDto);
    }

    [Fact]
    public async Task Update_Returns404_WhenNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new ReplaceUserRequest("updateduser", "updated@example.com");

        _svcMock.Setup(x => x.UpdateAsync(userId, request, default))
            .ReturnsAsync(Result<UserResponse>.NotFound("User not found"));

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/users/{userId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_Returns409_WhenConflict()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new ReplaceUserRequest("updateduser", "updated@example.com");

        _svcMock.Setup(x => x.UpdateAsync(userId, request, default))
            .ReturnsAsync(Result<UserResponse>.Conflict("Username or email already exists"));

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/users/{userId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    // DELETE /api/v1/users/{userId}

    [Fact]
    public async Task Delete_Returns204_WhenFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _svcMock.Setup(x => x.DeleteAsync(userId, default))
            .ReturnsAsync(Result<object>.Success(null));

        // Act
        var response = await _client.DeleteAsync($"/api/v1/users/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_Returns404_WhenNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _svcMock.Setup(x => x.DeleteAsync(userId, default))
            .ReturnsAsync(Result<object>.NotFound("User not found."));

        // Act
        var response = await _client.DeleteAsync($"/api/v1/users/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
