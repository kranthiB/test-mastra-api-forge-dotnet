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
  ApiForge.Domain -> /private/tmp/d799a719-dc22-42e0-be78-9e0b37cf4428/test-mastra-api-forge-dotnet/ApiForge.Domain/bin/Release/net10.0/ApiForge.Domain.dll
  ApiForge.Application -> /private/tmp/d799a719-dc22-42e0-be78-9e0b37cf4428/test-mastra-api-forge-dotnet/ApiForge.Application/bin/Release/net10.0/ApiForge.Application.dll
  ApiForge.Infrastructure -> /private/tmp/d799a719-dc22-42e0-be78-9e0b37cf4428/test-mastra-api-forge-dotnet/ApiForge.Infrastructure/bin/Release/net10.0/ApiForge.Infrastructure.dll
  ApiForge.Api -> /private/tmp/d799a719-dc22-42e0-be78-9e0b37cf4428/test-mastra-api-forge-dotnet/ApiForge.Api/bin/Release/net10.0/ApiForge.Api.dll
/private/tmp/d799a719-dc22-42e0-be78-9e0b37cf4428/test-mastra-api-forge-dotnet/ApiForge.Tests/Controllers/v1/GroupsControllerTests.cs(14,45): error CS0246: The type or namespace name 'IClassFixture<>' could not be found (are you missing a using directive or an assembly reference?) [/private/tmp/d799a719-dc22-42e0-be78-9e0b37cf4428/test-mastra-api-forge-dotnet/ApiForge.Tests/ApiForge.Tests.csproj]
/private/tmp/d799a719-dc22-42e0-be78-9e0b37cf4428/test-mastra-api-forge-dotnet/ApiForge.Tests/Controllers/v1/GroupsControllerTests.cs(30,6): error CS0246: The type or namespace name 'FactAttribute' could not be found (are you missing a using directive or an assembly reference?) [/private/tmp/d799a719-dc22-42e0-be78-9e0b37cf4428/test-mastra-api-forge-dotnet/ApiForge.Tests/ApiForge.Tests.csproj]
/private/tmp/d799a719-dc22-42e0-be78-9e0b37cf4428/test-mastra-api-forge-dotnet/ApiForge.Tests/Controllers/v1/GroupsControllerTests.cs(30,6): error CS0246: The type or namespace name 'Fact' could not be found (are you missing a using directive or an assembly reference?) [/private/tmp/d799a719-dc22-42e0-be78-9e0b37cf4428/test-mastra-api-forge-dotnet/ApiForge.Tests/ApiForge.Tests.csproj]
/private/tmp/d799a719-dc22-42e0-be78-9e0b37cf4428/test-mastra-api-forge-dotnet/Ap
```

---
*Approve to proceed to the next endpoint.*
*Reject with feedback to re-implement this endpoint incorporating your comments.*