
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ApiForge.Api;
using ApiForge.Application.Users.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ApiForge.Tests.Integration;

public sealed class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    // User Endpoints
    [Fact]
    [Trait("Category", "Integration")]
    public async Task PostUser_WithValidData_Returns201Created()
    {
        // Arrange
        var request = new CreateUserRequest($"testuser-{Guid.NewGuid()}@example.com", "Test", "User");

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/users", request);
        var user = await response.Content.ReadFromJsonAsync<UserResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Be($"/api/v1/users/{user!.Id}");
        user.Should().NotBeNull();
        user!.Email.Should().Be(request.Email);
        user.FirstName.Should().Be(request.FirstName);
        user.LastName.Should().Be(request.LastName);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task PostUser_WithDuplicateEmail_Returns409Conflict()
    {
        // Arrange
        var email = $"conflictuser-{Guid.NewGuid()}@example.com";
        var request = new CreateUserRequest(email, "Test", "User");
        await _client.PostAsJsonAsync("/api/v1/users", request); // First user

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/users", request); // Second user with same email

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task PostUser_WithInvalidData_Returns400BadRequest()
    {
        // Arrange
        var request = new CreateUserRequest("invalid-email", "", "");

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetUserById_WhenUserExists_Returns200Ok()
    {
        // Arrange
        var request = new CreateUserRequest($"getuser-{Guid.NewGuid()}@example.com", "Get", "Me");
        var createResponse = await _client.PostAsJsonAsync("/api/v1/users", request);
        var createdUser = await createResponse.Content.ReadFromJsonAsync<UserResponse>();

        // Act
        var response = await _client.GetAsync($"/api/v1/users/{createdUser!.Id}");
        var fetchedUser = await response.Content.ReadFromJsonAsync<UserResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        fetchedUser.Should().NotBeNull();
        fetchedUser!.Id.Should().Be(createdUser.Id);
        fetchedUser.Email.Should().Be(request.Email);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetUserById_WhenUserDoesNotExist_Returns404NotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/v1/users/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetUsers_Returns200Ok()
    {
        // Arrange
        await _client.PostAsJsonAsync("/api/v1/users", new CreateUserRequest($"listuser1-{Guid.NewGuid()}@example.com", "List", "One"));
        await _client.PostAsJsonAsync("/api/v1/users", new CreateUserRequest($"listuser2-{Guid.NewGuid()}@example.com", "List", "Two"));

        // Act
        var response = await _client.GetAsync("/api/v1/users?page=1&pageSize=10");
        var pagedResult = await response.Content.ReadFromJsonAsync<PagedResult<UserResponse>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        pagedResult.Should().NotBeNull();
        pagedResult!.Items.Should().NotBeEmpty();
        pagedResult.TotalCount.Should().BeGreaterOrEqualTo(2);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task PutUser_WhenUserExists_Returns200Ok()
    {
        // Arrange
        var createRequest = new CreateUserRequest($"updateuser-{Guid.NewGuid()}@example.com", "Original", "Name");
        var createResponse = await _client.PostAsJsonAsync("/api/v1/users", createRequest);
        var createdUser = await createResponse.Content.ReadFromJsonAsync<UserResponse>();
        var updateRequest = new UpdateUserRequest("Updated", "Name");

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/users/{createdUser!.Id}", updateRequest);
        var updatedUser = await response.Content.ReadFromJsonAsync<UserResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedUser.Should().NotBeNull();
        updatedUser!.Id.Should().Be(createdUser.Id);
        updatedUser.FirstName.Should().Be(updateRequest.FirstName);
        updatedUser.LastName.Should().Be(updateRequest.LastName);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task PutUser_WhenUserDoesNotExist_Returns404NotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var updateRequest = new UpdateUserRequest("Updated", "Name");

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/users/{nonExistentId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task DeleteUser_WhenUserExists_Returns204NoContent()
    {
        // Arrange
        var createRequest = new CreateUserRequest($"deleteuser-{Guid.NewGuid()}@example.com", "Delete", "Me");
        var createResponse = await _client.PostAsJsonAsync("/api/v1/users", createRequest);
        var createdUser = await createResponse.Content.ReadFromJsonAsync<UserResponse>();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/users/{createdUser!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion
        var getResponse = await _client.GetAsync($"/api/v1/users/{createdUser.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task DeleteUser_WhenUserDoesNotExist_Returns404NotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/users/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // Group Endpoints
    [Fact]
    [Trait("Category", "Integration")]
    public async Task PostGroup_WithValidData_Returns201Created()
    {
        // Arrange
        var request = new CreateGroupRequest($"Test Group {Guid.NewGuid()}", $"test-group-{Guid.NewGuid()}", "A test group.");

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/groups", request);
        var group = await response.Content.ReadFromJsonAsync<GroupResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Be($"/api/v1/groups/{group!.Slug}");
        group.Should().NotBeNull();
        group!.Name.Should().Be(request.Name);
        group.Slug.Should().Be(request.Slug);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task PostGroup_WithDuplicateSlug_Returns409Conflict()
    {
        // Arrange
        var slug = $"conflict-group-{Guid.NewGuid()}";
        var request = new CreateGroupRequest("Conflict Group", slug, "A test group.");
        await _client.PostAsJsonAsync("/api/v1/groups", request);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/groups", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetGroupBySlug_WhenGroupExists_Returns200Ok()
    {
        // Arrange
        var slug = $"get-group-{Guid.NewGuid()}";
        var request = new CreateGroupRequest("Get Group", slug, "A test group.");
        await _client.PostAsJsonAsync("/api/v1/groups", request);

        // Act
        var response = await _client.GetAsync($"/api/v1/groups/{slug}");
        var group = await response.Content.ReadFromJsonAsync<GroupResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        group.Should().NotBeNull();
        group!.Slug.Should().Be(slug);
        group.Name.Should().Be(request.Name);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetGroupBySlug_WhenGroupDoesNotExist_Returns404NotFound()
    {
        // Arrange
        var nonExistentSlug = $"non-existent-{Guid.NewGuid()}";

        // Act
        var response = await _client.GetAsync($"/api/v1/groups/{nonExistentSlug}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetGroups_Returns200Ok()
    {
        // Arrange
        await _client.PostAsJsonAsync("/api/v1/groups", new CreateGroupRequest($"List Group 1 {Guid.NewGuid()}", $"list-group-1-{Guid.NewGuid()}", ""));
        await _client.PostAsJsonAsync("/api/v1/groups", new CreateGroupRequest($"List Group 2 {Guid.NewGuid()}", $"list-group-2-{Guid.NewGuid()}", ""));

        // Act
        var response = await _client.GetAsync("/api/v1/groups?page=1&pageSize=10");
        var pagedResult = await response.Content.ReadFromJsonAsync<PagedResult<GroupResponse>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        pagedResult.Should().NotBeNull();
        pagedResult!.Items.Should().NotBeEmpty();
        pagedResult.TotalCount.Should().BeGreaterOrEqualTo(2);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task PutGroup_WhenGroupExists_Returns200Ok()
    {
        // Arrange
        var slug = $"update-group-{Guid.NewGuid()}";
        var createRequest = new CreateGroupRequest("Original Group", slug, "Original desc.");
        await _client.PostAsJsonAsync("/api/v1/groups", createRequest);
        var updateRequest = new UpdateGroupRequest("Updated Group", "Updated desc.");

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/groups/{slug}", updateRequest);
        var updatedGroup = await response.Content.ReadFromJsonAsync<GroupResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedGroup.Should().NotBeNull();
        updatedGroup!.Slug.Should().Be(slug);
        updatedGroup.Name.Should().Be(updateRequest.Name);
        updatedGroup.Description.Should().Be(updateRequest.Description);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task PutGroup_WhenGroupDoesNotExist_Returns404NotFound()
    {
        // Arrange
        var nonExistentSlug = $"non-existent-{Guid.NewGuid()}";
        var updateRequest = new UpdateGroupRequest("Updated Group", "Updated desc.");

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/groups/{nonExistentSlug}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task DeleteGroup_WhenGroupExists_Returns204NoContent()
    {
        // Arrange
        var slug = $"delete-group-{Guid.NewGuid()}";
        var createRequest = new CreateGroupRequest("Delete Group", slug, "Delete desc.");
        await _client.PostAsJsonAsync("/api/v1/groups", createRequest);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/groups/{slug}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion
        var getResponse = await _client.GetAsync($"/api/v1/groups/{slug}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task DeleteGroup_WhenGroupDoesNotExist_Returns404NotFound()
    {
        // Arrange
        var nonExistentSlug = $"non-existent-{Guid.NewGuid()}";

        // Act
        var response = await _client.DeleteAsync($"/api/v1/groups/{nonExistentSlug}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // GroupUserAssignment Endpoints
    [Fact]
    [Trait("Category", "Integration")]
    public async Task PostGroupUserAssignment_WithValidData_Returns201Created()
    {
        // Arrange
        var userResponse = await _client.PostAsJsonAsync("/api/v1/users", new CreateUserRequest($"assignuser-{Guid.NewGuid()}@example.com", "Assign", "User"));
        var user = await userResponse.Content.ReadFromJsonAsync<UserResponse>();

        var groupResponse = await _client.PostAsJsonAsync("/api/v1/groups", new CreateGroupRequest($"Assign Group {Guid.NewGuid()}", $"assign-group-{Guid.NewGuid()}", ""));
        var group = await groupResponse.Content.ReadFromJsonAsync<GroupResponse>();

        var request = new CreateGroupUserAssignmentRequest(user!.Id, group!.Id);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/group-user-assignments", request);
        var assignment = await response.Content.ReadFromJsonAsync<GroupUserAssignmentResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        assignment.Should().NotBeNull();
        assignment!.UserId.Should().Be(user.Id);
        assignment.GroupId.Should().Be(group.Id);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task PostGroupUserAssignment_WithInvalidUser_Returns400BadRequest()
    {
        // Arrange
        var groupResponse = await _client.PostAsJsonAsync("/api/v1/groups", new CreateGroupRequest($"Assign Group Invalid User {Guid.NewGuid()}", $"assign-group-invalid-user-{Guid.NewGuid()}", ""));
        var group = await groupResponse.Content.ReadFromJsonAsync<GroupResponse>();
        var request = new CreateGroupUserAssignmentRequest(Guid.NewGuid(), group!.Id);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/group-user-assignments", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task PostGroupUserAssignment_WithDuplicateAssignment_Returns409Conflict()
    {
        // Arrange
        var userResponse = await _client.PostAsJsonAsync("/api/v1/users", new CreateUserRequest($"assignuser-conflict-{Guid.NewGuid()}@example.com", "Assign", "User"));
        var user = await userResponse.Content.ReadFromJsonAsync<UserResponse>();

        var groupResponse = await _client.PostAsJsonAsync("/api/v1/groups", new CreateGroupRequest($"Assign Group Conflict {Guid.NewGuid()}", $"assign-group-conflict-{Guid.NewGuid()}", ""));
        var group = await groupResponse.Content.ReadFromJsonAsync<GroupResponse>();

        var request = new CreateGroupUserAssignmentRequest(user!.Id, group!.Id);
        await _client.PostAsJsonAsync("/api/v1/group-user-assignments", request); // First assignment

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/group-user-assignments", request); // Duplicate

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetGroupUserAssignmentById_WhenExists_Returns200Ok()
    {
        // Arrange
        var userResponse = await _client.PostAsJsonAsync("/api/v1/users", new CreateUserRequest($"getassignuser-{Guid.NewGuid()}@example.com", "Get", "Assign"));
        var user = await userResponse.Content.ReadFromJsonAsync<UserResponse>();
        var groupResponse = await _client.PostAsJsonAsync("/api/v1/groups", new CreateGroupRequest($"Get Assign Group {Guid.NewGuid()}", $"get-assign-group-{Guid.NewGuid()}", ""));
        var group = await groupResponse.Content.ReadFromJsonAsync<GroupResponse>();
        var createRequest = new CreateGroupUserAssignmentRequest(user!.Id, group!.Id);
        var createResponse = await _client.PostAsJsonAsync("/api/v1/group-user-assignments", createRequest);
        var createdAssignment = await createResponse.Content.ReadFromJsonAsync<GroupUserAssignmentResponse>();

        // Act
        var response = await _client.GetAsync($"/api/v1/group-user-assignments/{createdAssignment!.Id}");
        var fetchedAssignment = await response.Content.ReadFromJsonAsync<GroupUserAssignmentResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        fetchedAssignment.Should().NotBeNull();
        fetchedAssignment!.Id.Should().Be(createdAssignment.Id);
        fetchedAssignment.UserId.Should().Be(user.Id);
        fetchedAssignment.GroupId.Should().Be(group.Id);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetGroupUserAssignmentById_WhenDoesNotExist_Returns404NotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/v1/group-user-assignments/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetGroupUserAssignments_Returns200Ok()
    {
        // Arrange - Create a user and group to assign
        var userResponse = await _client.PostAsJsonAsync("/api/v1/users", new CreateUserRequest($"listassignuser-{Guid.NewGuid()}@example.com", "List", "Assign"));
        var user = await userResponse.Content.ReadFromJsonAsync<UserResponse>();
        var groupResponse = await _client.PostAsJsonAsync("/api/v1/groups", new CreateGroupRequest($"List Assign Group {Guid.NewGuid()}", $"list-assign-group-{Guid.NewGuid()}", ""));
        var group = await groupResponse.Content.ReadFromJsonAsync<GroupResponse>();
        await _client.PostAsJsonAsync("/api/v1/group-user-assignments", new CreateGroupUserAssignmentRequest(user!.Id, group!.Id));

        // Act
        var response = await _client.GetAsync("/api/v1/group-user-assignments?page=1&pageSize=10");
        var pagedResult = await response.Content.ReadFromJsonAsync<PagedResult<GroupUserAssignmentResponse>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        pagedResult.Should().NotBeNull();
        pagedResult!.Items.Should().NotBeEmpty();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task DeleteGroupUserAssignment_WhenExists_Returns204NoContent()
    {
        // Arrange
        var userResponse = await _client.PostAsJsonAsync("/api/v1/users", new CreateUserRequest($"delassignuser-{Guid.NewGuid()}@example.com", "Del", "Assign"));
        var user = await userResponse.Content.ReadFromJsonAsync<UserResponse>();
        var groupResponse = await _client.PostAsJsonAsync("/api/v1/groups", new CreateGroupRequest($"Del Assign Group {Guid.NewGuid()}", $"del-assign-group-{Guid.NewGuid()}", ""));
        var group = await groupResponse.Content.ReadFromJsonAsync<GroupResponse>();
        var createRequest = new CreateGroupUserAssignmentRequest(user!.Id, group!.Id);
        var createResponse = await _client.PostAsJsonAsync("/api/v1/group-user-assignments", createRequest);
        var createdAssignment = await createResponse.Content.ReadFromJsonAsync<GroupUserAssignmentResponse>();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/group-user-assignments/{createdAssignment!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion
        var getResponse = await _client.GetAsync($"/api/v1/group-user-assignments/{createdAssignment.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task DeleteGroupUserAssignment_WhenDoesNotExist_Returns404NotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/group-user-assignments/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

// Minimal DTOs for testing
public record CreateUserRequest(string Email, string FirstName, string LastName);
public record UpdateUserRequest(string FirstName, string LastName);
public record UserResponse(Guid Id, string Email, string FirstName, string LastName, DateTime CreatedAt, DateTime? UpdatedAt);

public record CreateGroupRequest(string Name, string Slug, string? Description);
public record UpdateGroupRequest(string Name, string? Description);
public record GroupResponse(Guid Id, string Name, string Slug, string? Description, DateTime CreatedAt, DateTime? UpdatedAt);

public record CreateGroupUserAssignmentRequest(Guid UserId, Guid GroupId);
public record GroupUserAssignmentResponse(Guid Id, Guid UserId, Guid GroupId, DateTime CreatedAt, DateTime? UpdatedAt);

public record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);
