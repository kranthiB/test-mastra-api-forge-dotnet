# Endpoint Review — `GET /users/{userId}`

**Status:** implemented
**Attempt:** 1

## Files

- **Source:** `ApiForge.Api/Controllers/v1/UsersController.cs`
- **Tests:** `ApiForge.Tests/Controllers/v1/UsersControllerTests.cs`

## Test Results

❌ **0 passed**, 1 failed, 0 skipped

```
  Determining projects to restore...
  All projects are up-to-date for restore.
  ApiForge.Domain -> /private/tmp/79dbfc9f-054b-41d9-84c7-85d608fb6ad3/test-mastra-api-forge-dotnet/ApiForge.Domain/bin/Release/net10.0/ApiForge.Domain.dll
/private/tmp/79dbfc9f-054b-41d9-84c7-85d608fb6ad3/test-mastra-api-forge-dotnet/ApiForge.Application/Users/Services/UserService.cs(8,35): error CS0535: 'UserService' does not implement interface member 'IUserService.GetByIdAsync(Guid, CancellationToken)' [/private/tmp/79dbfc9f-054b-41d9-84c7-85d608fb6ad3/test-mastra-api-forge-dotnet/ApiForge.Application/ApiForge.Application.csproj]
/private/tmp/79dbfc9f-054b-41d9-84c7-85d608fb6ad3/test-mastra-api-forge-dotnet/ApiForge.Application/Users/Services/UserService.cs(8,35): error CS0535: 'UserService' does not implement interface member 'IUserService.CreateAsync(CreateUserRequest, CancellationToken)' [/private/tmp/79dbfc9f-054b-41d9-84c7-85d608fb6ad3/test-mastra-api-forge-dotnet/ApiForge.Application/ApiForge.Application.csproj]

Build FAILED.

/private/tmp/79dbfc9f-054b-41d9-84c7-85d608fb6ad3/test-mastra-api-forge-dotnet/ApiForge.Application/Users/Services/UserService.cs(8,35): error CS0535: 'UserService' does not implement interface member 'IUserService.GetByIdAsync(Guid, CancellationToken)' [/private/tmp/79dbfc9f-054b-41d9-84c7-85d608fb6ad3/test-mastra-api-forge-dotnet/ApiForge.Application/ApiForge.Application.csproj]
/private/tmp/79dbfc9f-054b-41d9-84c7-85d608fb6ad3/test-mastra-api-forge-dotnet/ApiForge.Application/Users/Services/UserService.cs(8,35): error CS0535: 'UserService' does not implement interface member 'IUserService.CreateAsync(CreateUserRequest, CancellationToken)' [/private/tmp/79dbfc9f-054b-41d9-84c7-85d608fb6ad3/test-mastra-api-forge-dotnet/ApiForge.Application/ApiForge.Application.csproj]
    0 Warning(s)
    2 Error(s)

Time Elapsed 00:00:00.75


```

---
*Approve to proceed to the next endpoint.*
*Reject with feedback to re-implement this endpoint incorporating your comments.*