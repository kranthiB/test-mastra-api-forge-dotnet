
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
[SwaggerTag("Group-User Assignments")]
public sealed class GroupUserAssignmentsController : ControllerBase
{
    private readonly IGroupUserAssignmentService _service;

    public GroupUserAssignmentsController(IGroupUserAssignmentService service) => _service = service;

    [HttpGet]
    [SwaggerOperation(Summary = "List group-user assignments", OperationId = "GroupUserAssignments_List")]
    [ProducesResponseType(typeof(IReadOnlyList<GroupUserAssignmentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? groupId,
        [FromQuery] Guid? userId,
        CancellationToken ct = default)
    {
        var result = await _service.GetAllAsync(groupId, userId, ct);
        return Ok(result.Value);
    }

    [HttpGet("{groupUserAssignId:guid}", Name = "GetGroupUserAssignmentById")]
    [SwaggerOperation(Summary = "Get a group-user assignment by ID", OperationId = "GroupUserAssignments_GetById")]
    [ProducesResponseType(typeof(GroupUserAssignmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid groupUserAssignId, CancellationToken ct)
    {
        var r = await _service.GetByIdAsync(groupUserAssignId, ct);
        return r.IsSuccess ? Ok(r.Value) : Problem(r.Error, statusCode: 404);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Assign a user to a group", OperationId = "GroupUserAssignments_Create")]
    [ProducesResponseType(typeof(GroupUserAssignmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateGroupUserAssignmentRequest request,
        CancellationToken ct)
    {
        var result = await _service.CreateAsync(request, ct);

        if (!result.IsSuccess)
            return result.ErrorKind switch
            {
                ErrorType.Validation => ValidationProblem(result.Error!),
                ErrorType.NotFound => Problem(result.Error, statusCode: StatusCodes.Status404NotFound),
                ErrorType.Conflict => Problem(result.Error, statusCode: StatusCodes.Status409Conflict),
                _ => Problem(result.Error, statusCode: StatusCodes.Status500InternalServerError),
            };

        // The location header should point to the GET endpoint for the created resource.
        return CreatedAtRoute("GetGroupUserAssignmentById", new { groupUserAssignId = result.Value!.Id }, result.Value);
    }

    [HttpDelete("{groupUserAssignId:guid}")]
    [SwaggerOperation(Summary = "Unassign a user from a group", OperationId = "GroupUserAssignments_Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid groupUserAssignId, CancellationToken ct)
    {
        var r = await _service.DeleteAsync(groupUserAssignId, ct);
        return r.IsSuccess ? NoContent() : Problem(r.Error, statusCode: 404);
    }

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
