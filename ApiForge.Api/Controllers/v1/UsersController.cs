using ApiForge.Application.Common.Models;
using ApiForge.Application.Users.DTOs;
using ApiForge.Application.Users.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ApiForge.Api.Controllers.v1;

/// <summary>
/// Handles HTTP requests for the User resource.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
[SwaggerTag("Users")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service) => _service = service;

    // ── GET /api/v1/users ──────────────────────────────────────────────────

    /// <summary>
    /// Returns a paginated list of users.
    /// Corresponds to operationId: Users_List.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "List users (paged)", OperationId = "Users_List")]
    [ProducesResponseType(typeof(PagedResult<UserResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page     = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct     = default)
    {
        var result = await _service.GetAllAsync(page, pageSize, ct);
        return Ok(result.Value);
    }
}
