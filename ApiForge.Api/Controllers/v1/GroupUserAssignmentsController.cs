
using ApiForge.Application.Common.Models;
using ApiForge.Application.GroupUserAssignments.DTOs;
using ApiForge.Application.GroupUserAssignments.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ApiForge.Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/group-user-assignments")]
[Produces("application/json")]
[SwaggerTag("Group User Assignments")]
public sealed class GroupUserAssignmentsController : ControllerBase
{
    private readonly IGroupUserAssignmentService _service;

    public GroupUserAssignmentsController(IGroupUserAssignmentService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<GroupUserAssignmentResponse>), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int offset = 0, 
        [FromQuery] int limit = 25, 
        [FromQuery] string? groupSlug = null, 
        [FromQuery] Guid? userId = null, 
        CancellationToken cancellationToken = default)
    {
        if (limit < 1 || limit > 100)
        {
            return BadRequest("Limit must be between 1 and 100.");
        }
        if (offset < 0)
        {
            return BadRequest("Offset must be a non-negative number.");
        }

        var result = await _service.GetPaginatedAsync(offset, limit, groupSlug, userId, cancellationToken);

        return Ok(result.Value);
    }

    [HttpGet("{groupUserAssignId:guid}", Name = "GetGroupUserAssignmentById")]
    [ProducesResponseType(typeof(GroupUserAssignmentResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> GetAssignmentByIdAsync([FromRoute] Guid groupUserAssignId, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(groupUserAssignId, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return result.ErrorKind switch
        {
            ErrorType.NotFound => Problem(result.Error, statusCode: 404),
            _ => Problem(result.Error, statusCode: 500),
        };
    }

    [HttpPost]
    [ProducesResponseType(typeof(GroupUserAssignmentResponse), 201)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    public async Task<IActionResult> Create([FromBody] CreateGroupUserAssignmentRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(request, cancellationToken);

        if (!result.IsSuccess || result.Value == null)
        {
            return result.ErrorKind switch
            {
                ErrorType.Validation => BadRequest(new { result.Error }),
                ErrorType.Conflict => Problem(result.Error, statusCode: 409),
                _ => Problem(result.Error, statusCode: 500),
            };
        }

        return CreatedAtRoute(
            "GetGroupUserAssignmentById", 
            new { groupUserAssignId = result.Value.GroupUserAssignId }, 
            result.Value);
    }

    [HttpDelete("{groupUserAssignId:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> DeleteAssignmentAsync([FromRoute] Guid groupUserAssignId, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(groupUserAssignId, cancellationToken);
        if (result.IsSuccess)
        {
            return NoContent();
        }
        
        return result.ErrorKind switch
        {
            ErrorType.NotFound => Problem(result.Error, statusCode: 404),
            _ => Problem(result.Error, statusCode: 500),
        };
    }
}
