# Endpoint Review — `GET /groups`

**Status:** implemented
**Attempt:** 1

## Files

- **Source:** `ApiForge.Api/Controllers/v1/GroupsController.cs`
- **Tests:** `ApiForge.Tests/Controllers/v1/GroupsControllerTests.cs`

## Test Results

❌ **0 passed**, 1 failed, 0 skipped

```
  Determining projects to restore...
  All projects are up-to-date for restore.
  ApiForge.Domain -> /private/tmp/0a910a92-3ef9-4ad2-9e1d-da4a9d385c74/test-mastra-api-forge-dotnet/ApiForge.Domain/bin/Release/net10.0/ApiForge.Domain.dll
/private/tmp/0a910a92-3ef9-4ad2-9e1d-da4a9d385c74/test-mastra-api-forge-dotnet/ApiForge.Application/Groups/Services/GroupService.cs(23,51): error CS7036: There is no argument given that corresponds to the required parameter 'UpdatedAt' of 'GroupResponse.GroupResponse(Guid, string, string?, DateTime, DateTime?)' [/private/tmp/0a910a92-3ef9-4ad2-9e1d-da4a9d385c74/test-mastra-api-forge-dotnet/ApiForge.Application/ApiForge.Application.csproj]

Build FAILED.

/private/tmp/0a910a92-3ef9-4ad2-9e1d-da4a9d385c74/test-mastra-api-forge-dotnet/ApiForge.Application/Groups/Services/GroupService.cs(23,51): error CS7036: There is no argument given that corresponds to the required parameter 'UpdatedAt' of 'GroupResponse.GroupResponse(Guid, string, string?, DateTime, DateTime?)' [/private/tmp/0a910a92-3ef9-4ad2-9e1d-da4a9d385c74/test-mastra-api-forge-dotnet/ApiForge.Application/ApiForge.Application.csproj]
    0 Warning(s)
    1 Error(s)

Time Elapsed 00:00:00.38


```

---
*Approve to proceed to the next endpoint.*
*Reject with feedback to re-implement this endpoint incorporating your comments.*