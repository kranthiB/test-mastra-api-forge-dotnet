# Endpoint Review — `POST /groups`

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
  ApiForge.Application -> /private/tmp/0a910a92-3ef9-4ad2-9e1d-da4a9d385c74/test-mastra-api-forge-dotnet/ApiForge.Application/bin/Release/net10.0/ApiForge.Application.dll
  ApiForge.Infrastructure -> /private/tmp/0a910a92-3ef9-4ad2-9e1d-da4a9d385c74/test-mastra-api-forge-dotnet/ApiForge.Infrastructure/bin/Release/net10.0/ApiForge.Infrastructure.dll
  ApiForge.Api -> /private/tmp/0a910a92-3ef9-4ad2-9e1d-da4a9d385c74/test-mastra-api-forge-dotnet/ApiForge.Api/bin/Release/net10.0/ApiForge.Api.dll
/private/tmp/0a910a92-3ef9-4ad2-9e1d-da4a9d385c74/test-mastra-api-forge-dotnet/ApiForge.Tests/Application/Users/UserServiceTests.cs(52,9): warning CS8602: Dereference of a possibly null reference. [/private/tmp/0a910a92-3ef9-4ad2-9e1d-da4a9d385c74/test-mastra-api-forge-dotnet/ApiForge.Tests/ApiForge.Tests.csproj]
/private/tmp/0a910a92-3ef9-4ad2-9e1d-da4a9d385c74/test-mastra-api-forge-dotnet/ApiForge.Tests/Application/Groups/GroupServiceTests.cs(43,9): warning CS8602: Dereference of a possibly null reference. [/private/tmp/0a910a92-3ef9-4ad2-9e1d-da4a9d385c74/test-mastra-api-forge-dotnet/ApiForge.Tests/ApiForge.Tests.csproj]
/private/tmp/0a910a92-3ef9-4ad2-9e1d-da4a9d385c74/test-mastra-api-forge-dotnet/ApiForge.Tests/Application/Groups/GroupServiceTests.cs(59,38): error CS0103: The name 'Common' does not exist in the current context [/private/tmp/0a910a92-3ef9-4ad2-9e1d-da4a9d385c74/test-mastra-api-forge-dotnet/ApiForge.Tests/ApiForge.Tests.csproj]
/private/tmp/0a910a92-3ef9-4ad2-9e1d-da4a9d385c74/test-mastra-api-forge-dotnet/ApiForge.Tests/Application/Users/UserServiceTests.cs(70,9): warning CS8602: Dereference of a possibly null reference. [/private/tmp/0a910a92-3ef9-4ad2-9e1d-da4a9d385c74/test-mastra-api-forge-dotnet/ApiForge.Tests/ApiForge.Tes
```

---
*Approve to proceed to the next endpoint.*
*Reject with feedback to re-implement this endpoint incorporating your comments.*