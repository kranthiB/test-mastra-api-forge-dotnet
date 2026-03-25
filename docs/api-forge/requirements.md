# Requirements Analysis

**Domain:** unknown
**Endpoints:** 14

| Endpoint | Jira Ticket | Complexity |
|----------|-------------|------------|
| GET /users | SP-22 | medium |
| POST /users | SP-27 | medium |
| GET /users/{userId} | SP-22 | low |
| PUT /users/{userId} | UNTRACKED | medium |
| DELETE /users/{userId} | UNTRACKED | medium |
| GET /groups | UNTRACKED | medium |
| POST /groups | UNTRACKED | medium |
| GET /groups/{groupSlug} | UNTRACKED | low |
| PUT /groups/{groupSlug} | UNTRACKED | medium |
| DELETE /groups/{groupSlug} | UNTRACKED | medium |
| GET /group-user-assignments | UNTRACKED | medium |
| POST /group-user-assignments | UNTRACKED | medium |
| GET /group-user-assignments/{groupUserAssignId} | UNTRACKED | low |
| DELETE /group-user-assignments/{groupUserAssignId} | UNTRACKED | low |

---

```json
{
  "entries": [
    {
      "endpoint": "GET /users",
      "jiraTicketId": "SP-22",
      "userStory": "Offset-based pagination strategy (not cursor). Minimum complexity is medium per pagination rule. SP-22 is 'In Review'. DB backing: users table (SP-27, status Done). .NET mapping: controller action with [FromQuery] int offset = 0, [FromQuery] int limit = 25; service returns PagedResult<UserResponse>; FluentValidation rule enforces 1 ≤ limit ≤ 1000 and offset ≥ 0. Route: [Route(\"api/v{version:apiVersion}/users\")]. Requires [Authorize] + JWT Bearer middleware. REST client artefact delivery is part of this ticket's acceptance criteria.",
      "acceptanceCriteria": [
        "MUST return 200 OK with a list of users and default pagination (offset=0, limit=25) when no query parameters are provided",
        "MUST include userId, userName, email, cname, and _links fields on each user object in the response",
        "MUST include pagination metadata fields offset, limit, and total in the response envelope",
        "MUST include HATEOAS _links for navigation (self, next, prev) in the response envelope",
        "MUST return exactly the requested number of users (up to limit) starting from the given offset when offset and limit query parameters are provided",
        "MUST return _links.next pointing to offset+limit and _links.prev pointing to offset-limit when applicable pagination neighbours exist",
        "MUST return 200 OK with an empty items array and correct pagination metadata when the requested offset exceeds the total number of users",
        "MUST return 400 Bad Request with error code ADM400 and a detail message when limit is 0, negative, or exceeds the maximum of 1000",
        "MUST return 400 Bad Request with error code ADM400 when offset is a negative number",
        "MUST reject requests that do not include a valid Authorization: Bearer <token> header",
        "MUST reject requests that do not include the API-Version header per the API spec",
        "MUST provide a VS Code REST Client file named users-get.http at platform/sos/admin/rest-client/ that loads dotenv variables (TEST_HOST, TEST_API_VERSION, TEST_API_TOKEN) from a .env file in the same folder",
        "MUST include request examples in users-get.http for default listing, paged listing, and invalid parameter cases, each using Authorization: Bearer {{token}} and API-Version: {{apiVersion}} headers",
        "MUST include a .env.example file in the rest-client folder with example values for TEST_HOST, TEST_API_VERSION, and TEST_API_TOKEN",
        "MUST ensure the .env file is listed in .gitignore and is not committed to the repository"
      ],
      "dataModel": "{\"queryParams\":{\"offset\":\"int (default: 0, minimum: 0)\",\"limit\":\"int (default: 25, minimum: 1, maximum: 1000)\"},\"response\":{\"status\":200,\"schema\":\"PaginatedEnvelope<UserResponse> { offset: int, limit: int, total: int, items: UserResponse[], _links: { self: LinkObject, next?: LinkObject, prev?: LinkObject } } where UserResponse { userId: Guid, userName: string, email: string, cname: string?, _links: { self: LinkObject } }\"}}",
      "dependencies": [],
      "complexity": "medium",
      "notes": "Offset-based pagination strategy (not cursor). Minimum complexity is medium per pagination rule. SP-22 is 'In Review'. DB backing: users table (SP-27, status Done). .NET mapping: controller action with [FromQuery] int offset = 0, [FromQuery] int limit = 25; service returns PagedResult<UserResponse>; FluentValidation rule enforces 1 ≤ limit ≤ 1000 and offset ≥ 0. Route: [Route(\"api/v{version:apiVersion}/users\")]. Requires [Authorize] + JWT Bearer middleware. REST client artefact delivery is part of this ticket's acceptance criteria."
    },
    {
      "endpoint": "POST /users",
      "jiraTicketId": "SP-27",
      "userStory": "SP-27 is a DB migration story (status Done); acceptance criteria are derived from its technical details and test scenarios. Email uniqueness is enforced case-insensitively via a UNIQUE INDEX on lower(email). The cname is trigger-generated; the service layer must not attempt to set it directly. .NET mapping: [FromBody] CreateUserRequest record; FluentValidation for required fields and email format; service calls repository InsertAsync; maps Result<UserResponse>; returns CreatedAtAction on success. Requires [Authorize]. SP-27 comment confirms EF Core entity mapping is in scope.",
      "acceptanceCriteria": [
        "MUST persist a new user record to the users table with a DB-generated user_id (UUID), created_dts, and a null updated_dts on successful creation",
        "MUST return 201 Created with the created user resource including userId, userName, email, cname, and _links fields",
        "MUST reject creation if a user with the same email already exists (case-insensitive) and MUST return 409 Conflict",
        "MUST require userName and email fields in the request body; MUST return 400 Bad Request when either is missing",
        "MUST ensure the cname field is populated by the set_user_cname() PostgreSQL trigger using the app.company_slug session variable",
        "MUST reject requests that do not include a valid Authorization: Bearer <token> header",
        "MUST reject requests that do not include the API-Version header per the API spec"
      ],
      "dataModel": "{\"request\":{\"required\":[\"userName\",\"email\"],\"properties\":{\"userName\":\"string (max 100 chars)\",\"email\":\"string (email format, max 255 chars)\"}},\"response\":{\"status\":201,\"schema\":\"UserResponse { userId: Guid, userName: string, email: string, cname: string?, _links: { self: LinkObject, collection: LinkObject } }\"}}",
      "dependencies": [],
      "complexity": "medium",
      "notes": "SP-27 is a DB migration story (status Done); acceptance criteria are derived from its technical details and test scenarios. Email uniqueness is enforced case-insensitively via a UNIQUE INDEX on lower(email). The cname is trigger-generated; the service layer must not attempt to set it directly. .NET mapping: [FromBody] CreateUserRequest record; FluentValidation for required fields and email format; service calls repository InsertAsync; maps Result<UserResponse>; returns CreatedAtAction on success. Requires [Authorize]. SP-27 comment confirms EF Core entity mapping is in scope."
    },
    {
      "endpoint": "GET /users/{userId}",
      "jiraTicketId": "SP-22",
      "userStory": "SP-22 REST client indicative file includes GET single user (valid → 200), not-found (→ 404), and invalid UUID (→ 400) scenarios, confirming these three cases. .NET mapping: [FromRoute] Guid userId; model binding automatically returns 400 for non-UUID path values. Requires [Authorize].",
      "acceptanceCriteria": [
        "MUST return 200 OK with the full user resource (userId, userName, email, cname, _links) when a valid userId UUID is provided and the user exists",
        "MUST return 404 Not Found with error code ADM404 when no user exists for the given userId",
        "MUST return 400 Bad Request with error code ADM400 when the userId path parameter is not a valid UUID",
        "MUST reject requests that do not include a valid Authorization: Bearer <token> header",
        "MUST reject requests that do not include the API-Version header per the API spec"
      ],
      "dataModel": "{\"pathParams\":{\"userId\":\"Guid\"},\"response\":{\"status\":200,\"schema\":\"UserResponse { userId: Guid, userName: string, email: string, cname: string?, _links: { self: LinkObject, collection: LinkObject } }\"}}",
      "dependencies": [
        "createUser"
      ],
      "complexity": "low",
      "notes": "SP-22 REST client indicative file includes GET single user (valid → 200), not-found (→ 404), and invalid UUID (→ 400) scenarios, confirming these three cases. .NET mapping: [FromRoute] Guid userId; model binding automatically returns 400 for non-UUID path values. Requires [Authorize]."
    },
    {
      "endpoint": "PUT /users/{userId}",
      "jiraTicketId": "UNTRACKED",
      "userStory": "UNTRACKED — no Jira ticket found; acceptance criteria are spec-derived; verify with stakeholders. Full PUT semantics (replace, not patch). The set_user_cname() trigger will fire on UPDATE and refresh cname and updated_dts. .NET mapping: [FromRoute] Guid userId, [FromBody] ReplaceUserRequest record; FluentValidation for required fields; service calls UpdateAsync; Result<UserResponse>. Requires [Authorize].",
      "acceptanceCriteria": [
        "MUST return 200 OK with the updated user resource (userId, userName, email, cname, _links) when a valid userId is provided and the user exists",
        "MUST perform a full replacement of the user resource with the supplied request body fields",
        "MUST return 404 Not Found when no user exists for the given userId",
        "MUST return 400 Bad Request when the request body is missing required fields (userName, email)",
        "MUST return 400 Bad Request when the userId path parameter is not a valid UUID",
        "MUST return 409 Conflict when the replacement email is already in use by a different user (case-insensitive)",
        "MUST reject requests that do not include a valid Authorization: Bearer <token> header"
      ],
      "dataModel": "{\"pathParams\":{\"userId\":\"Guid\"},\"request\":{\"required\":[\"userName\",\"email\"],\"properties\":{\"userName\":\"string (max 100 chars)\",\"email\":\"string (email format, max 255 chars)\"}},\"response\":{\"status\":200,\"schema\":\"UserResponse { userId: Guid, userName: string, email: string, cname: string?, _links: { self: LinkObject, collection: LinkObject } }\"}}",
      "dependencies": [
        "createUser"
      ],
      "complexity": "medium",
      "notes": "UNTRACKED — no Jira ticket found; acceptance criteria are spec-derived; verify with stakeholders. Full PUT semantics (replace, not patch). The set_user_cname() trigger will fire on UPDATE and refresh cname and updated_dts. .NET mapping: [FromRoute] Guid userId, [FromBody] ReplaceUserRequest record; FluentValidation for required fields; service calls UpdateAsync; Result<UserResponse>. Requires [Authorize]."
    },
    {
      "endpoint": "DELETE /users/{userId}",
      "jiraTicketId": "UNTRACKED",
      "userStory": "UNTRACKED — no Jira ticket found; acceptance criteria are spec-derived; verify with stakeholders. SP-27 explicitly notes: 'Enforcement of group-assignment cleanup prior to DELETE is a separate behavior and not covered by this migration — see API notes.' This introduces a cross-aggregate dependency on group-user-assignments. .NET mapping: IActionResult return type; NoContent() on success; service must call ExistsByUserIdAsync on IGroupUserAssignmentRepository before deletion. Requires [Authorize].",
      "acceptanceCriteria": [
        "MUST return 204 No Content when the user is successfully deleted",
        "MUST return 404 Not Found when no user exists for the given userId",
        "MUST return 400 Bad Request when the userId path parameter is not a valid UUID",
        "MUST reject requests that do not include a valid Authorization: Bearer <token> header",
        "MUST NOT delete the user if active group-user assignments exist for that userId without first removing those assignments (or MUST return a 409 Conflict indicating dependent assignments exist)"
      ],
      "dataModel": "{\"pathParams\":{\"userId\":\"Guid\"},\"response\":{\"status\":204,\"schema\":\"No content body\"}}",
      "dependencies": [
        "createUser",
        "createGroupUserAssignment"
      ],
      "complexity": "medium",
      "notes": "UNTRACKED — no Jira ticket found; acceptance criteria are spec-derived; verify with stakeholders. SP-27 explicitly notes: 'Enforcement of group-assignment cleanup prior to DELETE is a separate behavior and not covered by this migration — see API notes.' This introduces a cross-aggregate dependency on group-user-assignments. .NET mapping: IActionResult return type; NoContent() on success; service must call ExistsByUserIdAsync on IGroupUserAssignmentRepository before deletion. Requires [Authorize]."
    },
    {
      "endpoint": "GET /groups",
      "jiraTicketId": "UNTRACKED",
      "userStory": "UNTRACKED — no Jira ticket found; acceptance criteria are spec-derived; verify with stakeholders. Offset-based pagination strategy, consistent with listUsers. .NET mapping: [FromQuery] int offset = 0, [FromQuery] int limit = 25; service returns PagedResult<GroupResponse>. Requires [Authorize].",
      "acceptanceCriteria": [
        "MUST return 200 OK with a paginated list of groups using default offset=0 and limit=25 when no query parameters are provided",
        "MUST include groupSlug, groupName, description, cname, and _links fields on each group object in the response",
        "MUST include pagination metadata fields offset, limit, and total in the response envelope",
        "MUST include HATEOAS _links (self, next, prev) in the response envelope",
        "MUST return 400 Bad Request with an error code when limit is 0, negative, or exceeds the maximum of 1000",
        "MUST return 400 Bad Request when offset is a negative number",
        "MUST return 200 OK with an empty items array when the offset exceeds the total number of groups",
        "MUST reject requests that do not include a valid Authorization: Bearer <token> header"
      ],
      "dataModel": "{\"queryParams\":{\"offset\":\"int (default: 0, minimum: 0)\",\"limit\":\"int (default: 25, minimum: 1, maximum: 1000)\"},\"response\":{\"status\":200,\"schema\":\"PaginatedEnvelope<GroupResponse> { offset: int, limit: int, total: int, items: GroupResponse[], _links: { self: LinkObject, next?: LinkObject, prev?: LinkObject } } where GroupResponse { groupSlug: string, groupName: string, description: string?, cname: string?, _links: { self: LinkObject } }\"}}",
      "dependencies": [],
      "complexity": "medium",
      "notes": "UNTRACKED — no Jira ticket found; acceptance criteria are spec-derived; verify with stakeholders. Offset-based pagination strategy, consistent with listUsers. .NET mapping: [FromQuery] int offset = 0, [FromQuery] int limit = 25; service returns PagedResult<GroupResponse>. Requires [Authorize]."
    },
    {
      "endpoint": "POST /groups",
      "jiraTicketId": "UNTRACKED",
      "userStory": "UNTRACKED — no Jira ticket found; acceptance criteria are spec-derived; verify with stakeholders. groupSlug is the natural key and path identifier for the groups resource. .NET mapping: [FromBody] CreateGroupRequest record; FluentValidation for required fields and slug format; service calls InsertAsync; returns CreatedAtAction. Requires [Authorize].",
      "acceptanceCriteria": [
        "MUST return 201 Created with the created group resource (groupSlug, groupName, description, cname, _links) on successful creation",
        "MUST require groupSlug and groupName in the request body; MUST return 400 Bad Request when either is missing",
        "MUST return 409 Conflict when a group with the same groupSlug already exists",
        "MUST reject requests that do not include a valid Authorization: Bearer <token> header"
      ],
      "dataModel": "{\"request\":{\"required\":[\"groupSlug\",\"groupName\"],\"properties\":{\"groupSlug\":\"string (URL-safe slug, unique identifier)\",\"groupName\":\"string\",\"description\":\"string? (optional)\"}},\"response\":{\"status\":201,\"schema\":\"GroupResponse { groupSlug: string, groupName: string, description: string?, cname: string?, _links: { self: LinkObject, collection: LinkObject } }\"}}",
      "dependencies": [],
      "complexity": "medium",
      "notes": "UNTRACKED — no Jira ticket found; acceptance criteria are spec-derived; verify with stakeholders. groupSlug is the natural key and path identifier for the groups resource. .NET mapping: [FromBody] CreateGroupRequest record; FluentValidation for required fields and slug format; service calls InsertAsync; returns CreatedAtAction. Requires [Authorize]."
    },
    {
      "endpoint": "GET /groups/{groupSlug}",
      "jiraTicketId": "UNTRACKED",
      "userStory": "UNTRACKED — no Jira ticket found; acceptance criteria are spec-derived; verify with stakeholders. groupSlug is a string path parameter ([FromRoute] string groupSlug). Requires [Authorize].",
      "acceptanceCriteria": [
        "MUST return 200 OK with the full group resource (groupSlug, groupName, description, cname, _links) when the groupSlug exists",
        "MUST return 404 Not Found when no group exists for the given groupSlug",
        "MUST reject requests that do not include a valid Authorization: Bearer <token> header"
      ],
      "dataModel": "{\"pathParams\":{\"groupSlug\":\"string\"},\"response\":{\"status\":200,\"schema\":\"GroupResponse { groupSlug: string, groupName: string, description: string?, cname: string?, _links: { self: LinkObject, collection: LinkObject } }\"}}",
      "dependencies": [
        "createGroup"
      ],
      "complexity": "low",
      "notes": "UNTRACKED — no Jira ticket found; acceptance criteria are spec-derived; verify with stakeholders. groupSlug is a string path parameter ([FromRoute] string groupSlug). Requires [Authorize]."
    },
    {
      "endpoint": "PUT /groups/{groupSlug}",
      "jiraTicketId": "UNTRACKED",
      "userStory": "UNTRACKED — no Jira ticket found; acceptance criteria are spec-derived; verify with stakeholders. groupSlug is immutable (it is the resource identifier); only groupName and description are replaceable. .NET mapping: [FromRoute] string groupSlug, [FromBody] ReplaceGroupRequest record. Requires [Authorize].",
      "acceptanceCriteria": [
        "MUST return 200 OK with the updated group resource (groupSlug, groupName, description, cname, _links) when the groupSlug exists and the request body is valid",
        "MUST perform a full replacement of the group resource with the supplied request body fields",
        "MUST return 404 Not Found when no group exists for the given groupSlug",
        "MUST return 400 Bad Request when the request body is missing required fields (groupName)",
        "MUST reject requests that do not include a valid Authorization: Bearer <token> header"
      ],
      "dataModel": "{\"pathParams\":{\"groupSlug\":\"string\"},\"request\":{\"required\":[\"groupName\"],\"properties\":{\"groupName\":\"string\",\"description\":\"string? (optional)\"}},\"response\":{\"status\":200,\"schema\":\"GroupResponse { groupSlug: string, groupName: string, description: string?, cname: string?, _links: { self: LinkObject, collection: LinkObject } }\"}}",
      "dependencies": [
        "createGroup"
      ],
      "complexity": "medium",
      "notes": "UNTRACKED — no Jira ticket found; acceptance criteria are spec-derived; verify with stakeholders. groupSlug is immutable (it is the resource identifier); only groupName and description are replaceable. .NET mapping: [FromRoute] string groupSlug, [FromBody] ReplaceGroupRequest record. Requires [Authorize]."
    },
    {
      "endpoint": "DELETE /groups/{groupSlug}",
      "jiraTicketId": "UNTRACKED",
      "userStory": "UNTRACKED — no Jira ticket found; acceptance criteria are spec-derived; verify with stakeholders. Cross-aggregate dependency on group-user-assignments (same pattern as deleteUser). .NET mapping: IActionResult return type; NoContent() on success; service must call ExistsByGroupSlugAsync on IGroupUserAssignmentRepository before deletion. Requires [Authorize].",
      "acceptanceCriteria": [
        "MUST return 204 No Content when the group is successfully deleted",
        "MUST return 404 Not Found when no group exists for the given groupSlug",
        "MUST reject requests that do not include a valid Authorization: Bearer <token> header",
        "MUST NOT delete the group if active group-user assignments exist for that groupSlug without first removing those assignments (or MUST return a 409 Conflict indicating dependent assignments exist)"
      ],
      "dataModel": "{\"pathParams\":{\"groupSlug\":\"string\"},\"response\":{\"status\":204,\"schema\":\"No content body\"}}",
      "dependencies": [
        "createGroup",
        "createGroupUserAssignment"
      ],
      "complexity": "medium",
      "notes": "UNTRACKED — no Jira ticket found; acceptance criteria are spec-derived; verify with stakeholders. Cross-aggregate dependency on group-user-assignments (same pattern as deleteUser). .NET mapping: IActionResult return type; NoContent() on success; service must call ExistsByGroupSlugAsync on IGroupUserAssignmentRepository before deletion. Requires [Authorize]."
    },
    {
      "endpoint": "GET /group-user-assignments",
      "jiraTicketId": "UNTRACKED",
      "userStory": "UNTRACKED — no Jira ticket found; acceptance criteria are spec-derived; verify with stakeholders. Offset-based pagination. This is a join/assignment resource — the repository requires cross-aggregate awareness of both users and groups. .NET mapping: [FromQuery] int offset = 0, [FromQuery] int limit = 25; service returns PagedResult<GroupUserAssignmentResponse>. Requires [Authorize].",
      "acceptanceCriteria": [
        "MUST return 200 OK with a paginated list of group-user assignments using default offset=0 and limit=25 when no query parameters are provided",
        "MUST include groupUserAssignId, groupSlug, userId, and _links fields on each assignment object in the response",
        "MUST include pagination metadata fields offset, limit, and total in the response envelope",
        "MUST include HATEOAS _links (self, next, prev) in the response envelope",
        "MUST return 400 Bad Request when limit is 0, negative, or exceeds the maximum of 1000",
        "MUST return 400 Bad Request when offset is a negative number",
        "MUST return 200 OK with an empty items array when the offset exceeds the total number of assignments",
        "MUST reject requests that do not include a valid Authorization: Bearer <token> header"
      ],
      "dataModel": "{\"queryParams\":{\"offset\":\"int (default: 0, minimum: 0)\",\"limit\":\"int (default: 25, minimum: 1, maximum: 1000)\"},\"response\":{\"status\":200,\"schema\":\"PaginatedEnvelope<GroupUserAssignmentResponse> { offset: int, limit: int, total: int, items: GroupUserAssignmentResponse[], _links: { self: LinkObject, next?: LinkObject, prev?: LinkObject } } where GroupUserAssignmentResponse { groupUserAssignId: Guid, groupSlug: string, userId: Guid, _links: { self: LinkObject } }\"}}",
      "dependencies": [
        "createGroup",
        "createUser"
      ],
      "complexity": "medium",
      "notes": "UNTRACKED — no Jira ticket found; acceptance criteria are spec-derived; verify with stakeholders. Offset-based pagination. This is a join/assignment resource — the repository requires cross-aggregate awareness of both users and groups. .NET mapping: [FromQuery] int offset = 0, [FromQuery] int limit = 25; service returns PagedResult<GroupUserAssignmentResponse>. Requires [Authorize]."
    },
    {
      "endpoint": "POST /group-user-assignments",
      "jiraTicketId": "UNTRACKED",
      "userStory": "UNTRACKED — no Jira ticket found; acceptance criteria are spec-derived; verify with stakeholders. This is a composite-key join resource (groupSlug + userId). Service must call ExistsByCompositeKeyAsync(groupSlug, userId) before insert to enforce 409. No PUT/PATCH endpoint exists — Update DTO and UpdateAsync are NOT needed. .NET mapping: [FromBody] CreateGroupUserAssignmentRequest record; FluentValidation for required fields; service validates existence of both parent resources before insert. Requires [Authorize].",
      "acceptanceCriteria": [
        "MUST return 201 Created with the created assignment resource (groupUserAssignId, groupSlug, userId, _links) on successful creation",
        "MUST require groupSlug and userId in the request body; MUST return 400 Bad Request when either is missing",
        "MUST return 404 Not Found when the referenced groupSlug does not exist",
        "MUST return 404 Not Found when the referenced userId does not exist",
        "MUST return 409 Conflict when an assignment for the same groupSlug and userId combination already exists",
        "MUST reject requests that do not include a valid Authorization: Bearer <token> header"
      ],
      "dataModel": "{\"request\":{\"required\":[\"groupSlug\",\"userId\"],\"properties\":{\"groupSlug\":\"string\",\"userId\":\"Guid\"}},\"response\":{\"status\":201,\"schema\":\"GroupUserAssignmentResponse { groupUserAssignId: Guid, groupSlug: string, userId: Guid, _links: { self: LinkObject, collection: LinkObject } }\"}}",
      "dependencies": [
        "createGroup",
        "createUser"
      ],
      "complexity": "medium",
      "notes": "UNTRACKED — no Jira ticket found; acceptance criteria are spec-derived; verify with stakeholders. This is a composite-key join resource (groupSlug + userId). Service must call ExistsByCompositeKeyAsync(groupSlug, userId) before insert to enforce 409. No PUT/PATCH endpoint exists — Update DTO and UpdateAsync are NOT needed. .NET mapping: [FromBody] CreateGroupUserAssignmentRequest record; FluentValidation for required fields; service validates existence of both parent resources before insert. Requires [Authorize]."
    },
    {
      "endpoint": "GET /group-user-assignments/{groupUserAssignId}",
      "jiraTicketId": "UNTRACKED",
      "userStory": "UNTRACKED — no Jira ticket found; acceptance criteria are spec-derived; verify with stakeholders. .NET mapping: [FromRoute] Guid groupUserAssignId; model binding returns 400 automatically for non-UUID values. Requires [Authorize].",
      "acceptanceCriteria": [
        "MUST return 200 OK with the full assignment resource (groupUserAssignId, groupSlug, userId, _links) when the groupUserAssignId exists",
        "MUST return 404 Not Found when no assignment exists for the given groupUserAssignId",
        "MUST return 400 Bad Request when the groupUserAssignId path parameter is not a valid UUID",
        "MUST reject requests that do not include a valid Authorization: Bearer <token> header"
      ],
      "dataModel": "{\"pathParams\":{\"groupUserAssignId\":\"Guid\"},\"response\":{\"status\":200,\"schema\":\"GroupUserAssignmentResponse { groupUserAssignId: Guid, groupSlug: string, userId: Guid, _links: { self: LinkObject, collection: LinkObject } }\"}}",
      "dependencies": [
        "createGroupUserAssignment"
      ],
      "complexity": "low",
      "notes": "UNTRACKED — no Jira ticket found; acceptance criteria are spec-derived; verify with stakeholders. .NET mapping: [FromRoute] Guid groupUserAssignId; model binding returns 400 automatically for non-UUID values. Requires [Authorize]."
    },
    {
      "endpoint": "DELETE /group-user-assignments/{groupUserAssignId}",
      "jiraTicketId": "UNTRACKED",
      "userStory": "UNTRACKED — no Jira ticket found; acceptance criteria are spec-derived; verify with stakeholders. No Update DTO or UpdateAsync needed (no PUT/PATCH on this resource). .NET mapping: IActionResult return type; NoContent() on success. Requires [Authorize]. This endpoint is a prerequisite for deleteUser and deleteGroup (assignment cleanup).",
      "acceptanceCriteria": [
        "MUST return 204 No Content when the group-user assignment is successfully deleted",
        "MUST return 404 Not Found when no assignment exists for the given groupUserAssignId",
        "MUST return 400 Bad Request when the groupUserAssignId path parameter is not a valid UUID",
        "MUST reject requests that do not include a valid Authorization: Bearer <token> header"
      ],
      "dataModel": "{\"pathParams\":{\"groupUserAssignId\":\"Guid\"},\"response\":{\"status\":204,\"schema\":\"No content body\"}}",
      "dependencies": [
        "createGroupUserAssignment"
      ],
      "complexity": "low",
      "notes": "UNTRACKED — no Jira ticket found; acceptance criteria are spec-derived; verify with stakeholders. No Update DTO or UpdateAsync needed (no PUT/PATCH on this resource). .NET mapping: IActionResult return type; NoContent() on success. Requires [Authorize]. This endpoint is a prerequisite for deleteUser and deleteGroup (assignment cleanup)."
    }
  ],
  "implementationOrder": [
    "GET /users",
    "POST /users",
    "GET /users/{userId}",
    "PUT /users/{userId}",
    "DELETE /users/{userId}",
    "GET /groups",
    "POST /groups",
    "GET /groups/{groupSlug}",
    "PUT /groups/{groupSlug}",
    "DELETE /groups/{groupSlug}",
    "GET /group-user-assignments",
    "POST /group-user-assignments",
    "GET /group-user-assignments/{groupUserAssignId}",
    "DELETE /group-user-assignments/{groupUserAssignId}"
  ],
  "sharedSchemas": [
    "UserResponse",
    "GroupResponse",
    "GroupUserAssignmentResponse",
    "PaginatedEnvelope",
    "ErrorResponse"
  ]
}
```
