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

---
*Approve to proceed with code generation. Reject with feedback to re-plan.*