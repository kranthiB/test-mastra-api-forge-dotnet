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

/// <summary>
/// Integration-style controller tests for <c>UsersController</c>.
/// The real ASP.NET Core pipeline is exercised via <see cref="WebApplicationFactory{TEntryPoint}"/>;
/// only the <see cref="IUserService"/> dependency is replaced with a mock.
/// </summary>
public sealed class UsersControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly Mock<IUserService> _svcMock = new();
    private readonly HttpClient _client;

    public UsersControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory
            .WithWebHostBuilder(b => b.ConfigureServices(services =>
                services.AddScoped<IUserService>(_ => _svcMock.Object)))
            .CreateClient();
    }

    // ── Helpers ────────────────────────────────────────────────────────────

    private static PagedResult<UserResponse> BuildPagedResult(
        int page = 1, int pageSize = 20, int totalCount = 0,
        IReadOnlyList<UserResponse>? items = null)
    {
        items ??= Array.Empty<UserResponse>();
        return new PagedResult<UserResponse>(items, totalCount, page, pageSize);
    }

    private static UserResponse SampleUser(Guid? id = null) => new(
        id ?? Guid.NewGuid(),
        "alice",
        "alice@example.com",
        "Alice Smith",
        true,
        DateTime.UtcNow,
        null);

    // ── GET /api/v1/users — 200 happy path ────────────────────────────────

    [Fact]
    public async Task GetUsers_Returns200_WithPagedResult_WhenUsersExist()
    {
        // Arrange
        var user   = SampleUser();
        var paged  = BuildPagedResult(page: 1, pageSize: 20, totalCount: 1, items: new[] { user });

        _svcMock
            .Setup(s => s.GetUsersAsync(1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PagedResult<UserResponse>>.Success(paged));

        // Act
        var response = await _client.GetAsync("/api/v1/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<PagedResult<UserResponse>>();
        body.Should().NotBeNull();
        body!.TotalCount.Should().Be(1);
        body.Items.Should().HaveCount(1);
        body.Page.Should().Be(1);
        body.PageSize.Should().Be(20);
    }

    // ── GET /api/v1/users — 200 with empty list ───────────────────────────

    [Fact]
    public async Task GetUsers_Returns200_WithEmptyList_WhenNoUsersExist()
    {
        // Arrange
        var paged = BuildPagedResult(page: 1, pageSize: 20, totalCount: 0);

        _svcMock
            .Setup(s => s.GetUsersAsync(1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PagedResult<UserResponse>>.Success(paged));

        // Act
        var response = await _client.GetAsync("/api/v1/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<PagedResult<UserResponse>>();
        body.Should().NotBeNull();
        body!.TotalCount.Should().Be(0);
        body.Items.Should().BeEmpty();
    }

    // ── GET /api/v1/users?page=2&pageSize=5 — pagination params forwarded ─

    [Fact]
    public async Task GetUsers_Returns200_WithCorrectPagination_WhenQueryParamsProvided()
    {
        // Arrange
        var users = Enumerable.Range(1, 5)
            .Select(_ => SampleUser())
            .ToList();
        var paged = BuildPagedResult(page: 2, pageSize: 5, totalCount: 15, items: users);

        _svcMock
            .Setup(s => s.GetUsersAsync(2, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PagedResult<UserResponse>>.Success(paged));

        // Act
        var response = await _client.GetAsync("/api/v1/users?page=2&pageSize=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<PagedResult<UserResponse>>();
        body.Should().NotBeNull();
        body!.Page.Should().Be(2);
        body.PageSize.Should().Be(5);
        body.TotalCount.Should().Be(15);
        body.Items.Should().HaveCount(5);
    }

    // ── GET /api/v1/users — default pagination params used ────────────────

    [Fact]
    public async Task GetUsers_Returns200_UsingDefaultPagination_WhenNoQueryParams()
    {
        // Arrange
        var paged = BuildPagedResult(page: 1, pageSize: 20, totalCount: 0);

        _svcMock
            .Setup(s => s.GetUsersAsync(1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PagedResult<UserResponse>>.Success(paged));

        // Act
        var response = await _client.GetAsync("/api/v1/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _svcMock.Verify(s => s.GetUsersAsync(1, 20, It.IsAny<CancellationToken>()), Times.Once);
    }

    // ── GET /api/v1/users — 500 server error ──────────────────────────────

    [Fact]
    public async Task GetUsers_Returns500_WhenServiceThrowsUnexpectedException()
    {
        // Arrange — use a non-mapped exception type so the middleware returns 500
        _svcMock
            .Setup(s => s.GetUsersAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ApplicationException("Unexpected storage failure"));

        // Act
        var response = await _client.GetAsync("/api/v1/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}
