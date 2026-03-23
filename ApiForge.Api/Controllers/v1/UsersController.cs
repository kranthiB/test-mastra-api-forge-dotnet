using ApiForge.Application.Common.Models;
using ApiForge.Application.Users.DTOs;
using ApiForge.Application.Users.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ApiForge.Api.Controllers.v1;

/// <summary>
/// Handles user management operations.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
[SwaggerTag("Users – user management")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service) => _service = service;

    // ── POST /api/v1/users ─────────────────────────────────────────────────

    /// <summary>Creates a new user.</summary>
    /// <remarks>operationId: Users_Create</remarks>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a user", OperationId = "Users_Create")]
    [ProducesResponseType(typeof(UserResponse),             StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails),           StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails),           StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken ct)
    {
        var result = await _service.CreateAsync(request, ct);

        if (!result.IsSuccess)
            return result.ErrorKind switch
            {
                ErrorType.Validation => ValidationProblem(result.Error!),
                ErrorType.Conflict   => Problem(result.Error, statusCode: StatusCodes.Status409Conflict),
                _                    => Problem(result.Error, statusCode: StatusCodes.Status500InternalServerError),
            };

        return CreatedAtAction(
            nameof(Create),
            new { id = result.Value!.UserId },
            result.Value);
    }

    // ── Helper ─────────────────────────────────────────────────────────────

    private IActionResult ValidationProblem(string detail)
    {
        var pd = new ValidationProblemDetails(ModelState)
        {
            Status = StatusCodes.Status400BadRequest,
            Detail = detail,
        };
        return BadRequest(pd);
    }
}
