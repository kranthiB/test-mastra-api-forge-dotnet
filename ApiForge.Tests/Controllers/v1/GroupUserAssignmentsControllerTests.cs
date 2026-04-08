
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
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
        {
            var serviceDescriptor = s.SingleOrDefault(d => d.ServiceType == typeof(IGroupUserAssignmentService));
            if (serviceDescriptor != null)
            {
                s.Remove(serviceDescriptor);
            }
            s.AddScoped<IGroupUserAssignmentService>(_ => _svcMock.Object);
        })).CreateClient();
    }

    [Fact]
    public async Task GetById_Returns200_WhenFound()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();
        var assignment = new GroupUserAssignmentResponse(assignmentId, Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, null);
        var result = Result<GroupUserAssignmentResponse>.Success(assignment);

        _svcMock.Setup(s => s.GetByIdAsync(assignmentId, default)).ReturnsAsync(result);

        // Act
        var response = await _client.GetAsync($"/api/v1/group-user-assignments/{assignmentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<GroupUserAssignmentResponse>();
        content.Should().NotBeNull();
        content.Should().BeEquivalentTo(assignment);
    }

    [Fact]
    public async Task GetById_Returns404_WhenNotFound()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();
        var result = Result<GroupUserAssignmentResponse>.NotFound("not found");

        _svcMock.Setup(s => s.GetByIdAsync(assignmentId, default)).ReturnsAsync(result);

        // Act
        var response = await _client.GetAsync($"/api/v1/group-user-assignments/{assignmentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAll_Returns200_WithAssignments()
    {
        // Arrange
        var assignments = new List<GroupUserAssignmentResponse>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, null),
            new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, null)
        };
        var result = Result<IReadOnlyList<GroupUserAssignmentResponse>>.Success(assignments);

        _svcMock.Setup(s => s.GetAllAsync(null, null, default)).ReturnsAsync(result);

        // Act
        var response = await _client.GetAsync("/api/v1/group-user-assignments");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<IReadOnlyList<GroupUserAssignmentResponse>>();
        content.Should().NotBeNull();
        content.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAll_WithGroupId_Returns200_WithFilteredAssignments()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var assignments = new List<GroupUserAssignmentResponse>
        {
            new(Guid.NewGuid(), groupId, Guid.NewGuid(), DateTime.UtcNow, null)
        };
        var result = Result<IReadOnlyList<GroupUserAssignmentResponse>>.Success(assignments);

        _svcMock.Setup(s => s.GetAllAsync(groupId, null, default)).ReturnsAsync(result);

        // Act
        var response = await _client.GetAsync($"/api/v1/group-user-assignments?groupId={groupId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<IReadOnlyList<GroupUserAssignmentResponse>>();
        content.Should().NotBeNull();
        content.Should().HaveCount(1);
    }

    [Fact]
    public async Task Create_Returns201_WhenValid()
    {
        // Arrange
        var request = new CreateGroupUserAssignmentRequest(Guid.NewGuid(), Guid.NewGuid());
        var responseDto = new GroupUserAssignmentResponse(Guid.NewGuid(), request.GroupId, request.UserId, DateTime.UtcNow, null);

        _svcMock.Setup(x => x.CreateAsync(It.IsAny<CreateGroupUserAssignmentRequest>(), default))
                .ReturnsAsync(Result<GroupUserAssignmentResponse>.Success(responseDto));

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/group-user-assignments", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdAssignment = await response.Content.ReadFromJsonAsync<GroupUserAssignmentResponse>();
        createdAssignment.Should().BeEquivalentTo(responseDto);
        response.Headers.Location.Should().Be($"/api/v1/group-user-assignments/{responseDto.Id}");
    }

    [Fact]
    public async Task Create_Returns400_WhenInvalid()
    {
        // Arrange
        var request = new CreateGroupUserAssignmentRequest(Guid.Empty, Guid.Empty);

        _svcMock.Setup(x => x.CreateAsync(It.IsAny<CreateGroupUserAssignmentRequest>(), default))
            .ReturnsAsync(Result<GroupUserAssignmentResponse>.Validation("validation failed"));

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/group-user-assignments", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_Returns404_WhenUserNotFound()
    {
        // Arrange
        var request = new CreateGroupUserAssignmentRequest(Guid.NewGuid(), Guid.NewGuid());

        _svcMock.Setup(x => x.CreateAsync(It.IsAny<CreateGroupUserAssignmentRequest>(), default))
            .ReturnsAsync(Result<GroupUserAssignmentResponse>.NotFound("user not found"));

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/group-user-assignments", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_Returns409_WhenAssignmentExists()
    {
        // Arrange
        var request = new CreateGroupUserAssignmentRequest(Guid.NewGuid(), Guid.NewGuid());

        _svcMock.Setup(x => x.CreateAsync(It.IsAny<CreateGroupUserAssignmentRequest>(), default))
            .ReturnsAsync(Result<GroupUserAssignmentResponse>.Conflict("already exists"));

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/group-user-assignments", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Delete_Returns204_WhenFound()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();
        _svcMock.Setup(s => s.DeleteAsync(assignmentId, default))
            .ReturnsAsync(Result<object?>.Success(null));

        // Act
        var response = await _client.DeleteAsync($"/api/v1/group-user-assignments/{assignmentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_Returns404_WhenNotFound()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();
        _svcMock.Setup(s => s.DeleteAsync(assignmentId, default))
            .ReturnsAsync(Result<object?>.NotFound("not found"));

        // Act
        var response = await _client.DeleteAsync($"/api/v1/group-user-assignments/{assignmentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
