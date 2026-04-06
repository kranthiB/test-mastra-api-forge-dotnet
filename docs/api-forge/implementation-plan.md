# Implementation Plan

**Stack:** .NET ASP.NET Core
**Endpoints to implement:** 14

## Endpoint → File Mapping

| Endpoint | Target File | Test File |
|----------|-------------|-----------|
| `GET /users` | `ApiForge.Api/Controllers/v1/UsersController.cs` | `ApiForge.Tests/Controllers/v1/UsersControllerTests.cs` |
| `POST /users` | `ApiForge.Api/Controllers/v1/UsersController.cs` | `ApiForge.Tests/Controllers/v1/UsersControllerTests.cs` |
| `GET /users/{userId}` | `ApiForge.Api/Controllers/v1/UsersController.cs` | `ApiForge.Tests/Controllers/v1/UsersControllerTests.cs` |
| `PUT /users/{userId}` | `ApiForge.Api/Controllers/v1/UsersController.cs` | `ApiForge.Tests/Controllers/v1/UsersControllerTests.cs` |
| `DELETE /users/{userId}` | `ApiForge.Api/Controllers/v1/UsersController.cs` | `ApiForge.Tests/Controllers/v1/UsersControllerTests.cs` |
| `GET /groups` | `ApiForge.Api/Controllers/v1/GroupsController.cs` | `ApiForge.Tests/Controllers/v1/GroupsControllerTests.cs` |
| `POST /groups` | `ApiForge.Api/Controllers/v1/GroupsController.cs` | `ApiForge.Tests/Controllers/v1/GroupsControllerTests.cs` |
| `GET /groups/{groupSlug}` | `ApiForge.Api/Controllers/v1/GroupsController.cs` | `ApiForge.Tests/Controllers/v1/GroupsControllerTests.cs` |
| `PUT /groups/{groupSlug}` | `ApiForge.Api/Controllers/v1/GroupsController.cs` | `ApiForge.Tests/Controllers/v1/GroupsControllerTests.cs` |
| `DELETE /groups/{groupSlug}` | `ApiForge.Api/Controllers/v1/GroupsController.cs` | `ApiForge.Tests/Controllers/v1/GroupsControllerTests.cs` |
| `GET /group-user-assignments` | `ApiForge.Api/Controllers/v1/GroupUserAssignmentsController.cs` | `ApiForge.Tests/Controllers/v1/GroupUserAssignmentsControllerTests.cs` |
| `POST /group-user-assignments` | `ApiForge.Api/Controllers/v1/GroupUserAssignmentsController.cs` | `ApiForge.Tests/Controllers/v1/GroupUserAssignmentsControllerTests.cs` |
| `GET /group-user-assignments/{groupUserAssignId}` | `ApiForge.Api/Controllers/v1/GroupUserAssignmentsController.cs` | `ApiForge.Tests/Controllers/v1/GroupUserAssignmentsControllerTests.cs` |
| `DELETE /group-user-assignments/{groupUserAssignId}` | `ApiForge.Api/Controllers/v1/GroupUserAssignmentsController.cs` | `ApiForge.Tests/Controllers/v1/GroupUserAssignmentsControllerTests.cs` |

## Shared Files
- `ApiForge.Application/Users/DTOs/CreateUserRequest.cs`
- `ApiForge.Application/Users/Services/UserService.cs`
- `ApiForge.Domain/Users/User.cs`
- `ApiForge.Application/Users/Validators/CreateUserRequestValidator.cs`
- `ApiForge.Application/Users/Interfaces/IUserRepository.cs`
- `ApiForge.Tests/Application/Users/UserServiceTests.cs`
- `ApiForge.Tests/Infrastructure/UsersRepositoryTests.cs`
- `ApiForge.Application/Users/Interfaces/IUserService.cs`
- `ApiForge.Application/Groups/DTOs/CreateGroupRequest.cs`
- `ApiForge.Application/Groups/Services/GroupService.cs`
- `ApiForge.Domain/Groups/Group.cs`
- `ApiForge.Application/Groups/Validators/CreateGroupRequestValidator.cs`
- `ApiForge.Application/Groups/Interfaces/IGroupRepository.cs`
- `ApiForge.Tests/Application/Groups/GroupServiceTests.cs`
- `ApiForge.Tests/Infrastructure/GroupsRepositoryTests.cs`
- `ApiForge.Application/Groups/Interfaces/IGroupService.cs`
- `ApiForge.Application/GroupUserAssignments/DTOs/CreateGroupUserAssignmentRequest.cs`
- `ApiForge.Application/GroupUserAssignments/Services/GroupUserAssignmentService.cs`
- `ApiForge.Domain/GroupUserAssignments/GroupUserAssignment.cs`
- `ApiForge.Application/GroupUserAssignments/Validators/CreateGroupUserAssignmentRequestValidator.cs`
- `ApiForge.Application/GroupUserAssignments/Interfaces/IGroupUserAssignmentRepository.cs`
- `ApiForge.Tests/Application/GroupUserAssignments/GroupUserAssignmentServiceTests.cs`
- `ApiForge.Tests/Infrastructure/GroupUserAssignmentsRepositoryTests.cs`
- `ApiForge.Application/GroupUserAssignments/Interfaces/IGroupUserAssignmentService.cs`

---
*Approve to proceed with code generation. Reject with feedback to re-plan.*