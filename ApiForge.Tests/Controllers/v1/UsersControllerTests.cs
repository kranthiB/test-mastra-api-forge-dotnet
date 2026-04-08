
using System.Net;
using System.Net.Http.Json;
using ApiForge.Application.Common.Models;
using ApiForge.Application.Users.DTOs;
using ApiForge.Application.Users.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

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

    [Fact]
    public async Task GetAll_Returns200_WithCorrectOffsetAndLimit()
    {
        // Arrange
        var users = new List<UserResponse>
        {
            new(Guid.NewGuid(), "user3", "user3@test.com", null, DateTime.UtcNow, null)
        };
        var pagedResult = new OffsetPagedResult<UserResponse>(users, 5, 2, 2);
        var result = Result<OffsetPagedResult<UserResponse>>.Success(pagedResult);

        _svcMock.Setup(s => s.GetAllAsync(2, 2, default)).ReturnsAsync(result);

        // Act
        var response = await _client.GetAsync("/api/v1/users?offset=2&limit=2");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<OffsetPagedResult<UserResponse>>();
        content.Should().NotBeNull();
        content!.Items.Should().HaveCount(1);
        content.Offset.Should().Be(2);
        content.Limit.Should().Be(2);
    }

    [Fact]
    public async Task GetAll_Returns200_WithEmptyList_WhenOffsetExceedsTotal()
    {
        // Arrange
        var users = new List<UserResponse>();
        var pagedResult = new OffsetPagedResult<UserResponse>(users, 3, 10, 5);
        var result = Result<OffsetPagedResult<UserResponse>>.Success(pagedResult);

        _svcMock.Setup(s => s.GetAllAsync(10, 5, default)).ReturnsAsync(result);

        // Act
        var response = await _client.GetAsync("/api/v1/users?offset=10&limit=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<OffsetPagedResult<UserResponse>>();
        content.Should().NotBeNull();
        content!.Items.Should().BeEmpty();
        content.Total.Should().Be(3);
    }
}
