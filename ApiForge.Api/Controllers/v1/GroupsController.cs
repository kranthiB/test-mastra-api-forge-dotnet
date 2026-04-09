
using ApiForge.Application.Common.Models;
using ApiForge.Application.Groups.DTOs;
using ApiForge.Application.Groups.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ApiForge.Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/groups")]
[Produces("application/json")]
[SwaggerTag("Groups")]
public sealed class GroupsController : ControllerBase
{
    private readonly IGroupService _groupService;
    private readonly ILogger<GroupsController> _logger;

    public GroupsController(IGroupService groupService, ILogger<GroupsController> logger)
    {
        _groupService = groupService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new group.
    /// </summary>
    /// <param name="request">The request body for creating a group.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created group.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(GroupResponse), 201)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IActionResult> CreateGroupAsync([FromBody] CreateGroupRequest request, CancellationToken cancellationToken)
    {
        var result = await _groupService.CreateAsync(request, cancellationToken);

        if (result.IsSuccess)
        {
            // The location header should point to a "GetById" endpoint, which is not yet implemented.
            // For now, we will return the created object without the location header.
            return Created($"/api/v1/groups/{result.Value!.Id}", result.Value);
        }

        return result.ErrorKind switch
        {
            ErrorType.Validation => BadRequest(new { result.Error }),
            ErrorType.Conflict => Problem(result.Error, statusCode: 409),
            _ => Problem(result.Error, statusCode: 500),
        };
    }

    /// <summary>
    /// Returns a paginated list of groups.
    /// </summary>
    /// <param name="offset">The number of items to skip.</param>
    /// <param name="limit">The maximum number of items to return.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated list of groups.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<GroupResponse>), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public async Task<IActionResult> GetGroupsAsync([FromQuery] int offset = 0, [FromQuery] int limit = 25, CancellationToken cancellationToken = default)
    {
        if (limit < 1 || limit > 100)
        {
            return BadRequest("Limit must be between 1 and 100.");
        }

        var result = await _groupService.GetPaginatedAsync(offset, limit, cancellationToken);

        return Ok(result.Value);
    }

    [HttpGet("{groupSlug}", Name = "GetGroupBySlug")]
    [SwaggerOperation(Summary = "Get a group by its slug", OperationId = "Groups_GetBySlug")]
    [ProducesResponseType(typeof(GroupResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGroupAsync([FromRoute] string groupSlug, CancellationToken cancellationToken)
    {
        var result = await _groupService.GetBySlugAsync(groupSlug, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return result.ErrorKind switch
        {
            ErrorType.NotFound => Problem(result.Error, statusCode: StatusCodes.Status404NotFound),
            _ => Problem(result.Error, statusCode: 500),
        };
    }

    /// <summary>
    /// Replaces an existing group.
    /// </summary>
    /// <param name="groupSlug">The slug of the group to replace.</param>
    /// <param name="request">The request body for replacing a group.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated group.</returns>
    [HttpPut("{groupSlug}")]
    [ProducesResponseType(typeof(GroupResponse), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IActionResult> ReplaceGroupAsync([FromRoute] string groupSlug, [FromBody] UpdateGroupRequest request, CancellationToken cancellationToken)
    {
        var result = await _groupService.UpdateAsync(groupSlug, request, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return result.ErrorKind switch
        {
            ErrorType.Validation => BadRequest(new { result.Error }),
            ErrorType.NotFound => Problem(result.Error, statusCode: 404),
            ErrorType.Conflict => Problem(result.Error, statusCode: 409),
            _ => Problem(result.Error, statusCode: 500),
        };
    }

    [HttpDelete("{groupSlug}")]
    [SwaggerOperation(Summary = "Delete a group by its slug", OperationId = "Groups_DeleteBySlug")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteGroupAsync([FromRoute] string groupSlug, CancellationToken cancellationToken)
    {
        var result = await _groupService.DeleteBySlugAsync(groupSlug, cancellationToken);

        if (result.IsSuccess)
        {
            return NoContent();
        }

        return result.ErrorKind switch
        {
            ErrorType.NotFound => Problem(result.Error, statusCode: StatusCodes.Status404NotFound),
            ErrorType.Conflict => Problem(result.Error, statusCode: StatusCodes.Status409Conflict),
            _ => Problem(result.Error, statusCode: 500),
        };
    }
}
