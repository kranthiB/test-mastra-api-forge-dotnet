using ApiForge.Application.Common.Models;
using ApiForge.Application.Users.DTOs;
using ApiForge.Application.Users.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ApiForge.Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
[Produces("application/json")]
[SwaggerTag("Users")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "List users with offset pagination", OperationId = "Users_List")]
    [ProducesResponseType(typeof(OffsetPagedResult<UserResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int offset = 0,
        [FromQuery] int limit = 25,
        CancellationToken ct = default)
    {
        var result = await _userService.GetUsersAsync(offset, limit, ct);
        return Ok(result.Value);
    }

    /// <summary>Returns a single user by its ID.</summary>
    [HttpGet("{userId:guid}", Name = "GetUserById")]
    [SwaggerOperation(Summary = "Get a user by ID", OperationId = "Users_GetById")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),  StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid userId, CancellationToken ct)
    {
        var result = await _userService.GetByIdAsync(userId, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : Problem(result.Error, statusCode: StatusCodes.Status404NotFound);
    }

    /// <summary>Creates a new user.</summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a user", OperationId = "Users_Create")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails),           StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken ct)
    {
        var result = await _userService.CreateAsync(request, ct);

        if (!result.IsSuccess)
            return result.ErrorKind switch
            {
                ErrorType.Validation => Problem(result.Error, statusCode: StatusCodes.Status400BadRequest),
                ErrorType.Conflict   => Problem(result.Error, statusCode: StatusCodes.Status409Conflict),
                _                    => Problem(result.Error, statusCode: StatusCodes.Status500InternalServerError),
            };

        return CreatedAtRoute(
            "GetUserById",
            new { userId = result.Value!.UserId },
            result.Value);
    }
}
