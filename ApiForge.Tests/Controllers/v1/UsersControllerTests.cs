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
/// The real ASP.NET Core pipeline is exercised via <see cref="WebApplicationFactory{TEntryPoint}"/>.
/// All external dependencies (IUserService) are replaced with Moq doubles.
/// </summary>
public sealed class UsersControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public UsersControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    // ── Helpers ────────────────────────────────────────────────────────────

    private HttpClient CreateClientWithService(Mock<IUserService> svcMock)
    {
        return _factory.WithWebHostBuilder(b =>
            b.ConfigureServices(services =>
                services.AddScoped<IUserService>(_ => svcMock.Object)))
            .CreateClient();
    }

    private static PagedResult<UserResponse> EmptyPage(int page = 1, int pageSize = 10) =>
        new(Array.Empty<UserResponse>(), 0, page, pageSize);

    private static PagedResult<UserResponse> SingleUserPage() =>
        new(
            new[]
            {
                new UserResponse(
                    Guid.NewGuid(),
                    "alice",
                    "alice@example.com",
                    DateTime.UtcNow,
                    null)
            },
            1, 1, 10);

    // ── Happy path ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAll_Returns200_WithPagedResult()
    {
        // Arrange
        var svcMock = new Mock<IUserService>();
        svcMock
            .Setup(s => s.GetAllAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PagedResult<UserResponse>>.Success(SingleUserPage()));

        var client = CreateClientWithService(svcMock);

        // Act
        var response = await client.GetAsync("/api/v1/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<PagedResult<UserResponse>>();
        body.Should().NotBeNull();
        body!.Items.Should().HaveCount(1);
        body.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task GetAll_Returns200_WithEmptyList_WhenNoUsersExist()
    {
        // Arrange
        var svcMock = new Mock<IUserService>();
        svcMock
            .Setup(s => s.GetAllAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PagedResult<UserResponse>>.Success(EmptyPage()));

        var client = CreateClientWithService(svcMock);

        // Act
        var response = await client.GetAsync("/api/v1/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<PagedResult<UserResponse>>();
        body.Should().NotBeNull();
        body!.Items.Should().BeEmpty();
        body.TotalCount.Should().Be(0);
    }

    // ── Pagination ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAll_ForwardsPaginationParams_ToService()
    {
        // Arrange
        var svcMock = new Mock<IUserService>();
        svcMock
            .Setup(s => s.GetAllAsync(2, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PagedResult<UserResponse>>.Success(EmptyPage(2, 5)));

        var client = CreateClientWithService(svcMock);

        // Act
        var response = await client.GetAsync("/api/v1/users?page=2&pageSize=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        svcMock.Verify(s => s.GetAllAsync(2, 5, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAll_Returns200_WithDefaultPagination_WhenNoParamsProvided()
    {
        // Arrange
        var svcMock = new Mock<IUserService>();
        svcMock
            .Setup(s => s.GetAllAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PagedResult<UserResponse>>.Success(EmptyPage()));

        var client = CreateClientWithService(svcMock);

        // Act
        var response = await client.GetAsync("/api/v1/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        svcMock.Verify(s => s.GetAllAsync(1, 10, It.IsAny<CancellationToken>()), Times.Once);
    }

    // ── Response shape ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetAll_Returns200_WithCorrectPagedResultShape()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var pagedResult = new PagedResult<UserResponse>(
            new[] { new UserResponse(userId, "bob", "bob@example.com", createdAt, null) },
            1,
            1,
            10);

        var svcMock = new Mock<IUserService>();
        svcMock
            .Setup(s => s.GetAllAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PagedResult<UserResponse>>.Success(pagedResult));

        var client = CreateClientWithService(svcMock);

        // Act
        var response = await client.GetAsync("/api/v1/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<PagedResult<UserResponse>>();
        body.Should().NotBeNull();
        body!.Page.Should().Be(1);
        body.PageSize.Should().Be(10);
        body.TotalCount.Should().Be(1);
        body.Items.Should().HaveCount(1);
        body.Items[0].UserName.Should().Be("bob");
        body.Items[0].Email.Should().Be("bob@example.com");
    }

    // ── Server error ───────────────────────────────────────────────────────

    [Fact]
    public async Task GetAll_Returns500_WhenServiceThrowsUnexpectedException()
    {
        // Arrange
        // Use a non-mapped exception type so the middleware's catch-all returns 500.
        // (ArgumentException / InvalidOperationException are mapped to 400 by the middleware.)
        var svcMock = new Mock<IUserService>();
        svcMock
            .Setup(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ApplicationException("Unexpected failure"));

        var client = CreateClientWithService(svcMock);

        // Act
        var response = await client.GetAsync("/api/v1/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}
