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

    public GroupUserAssignmentsController(IGroupUserAssignmentService service) => _service = service;

    [HttpGet("{id:guid}", Name = "GetGroupUserAssignmentById")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        return result.IsSuccess ? Ok(result.Value) : Problem(result.Error, statusCode: 404);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGroupUserAssignmentRequest request, CancellationToken ct)
    {
        var result = await _service.CreateAsync(request, ct);
        if (!result.IsSuccess)
        {
            return result.ErrorKind switch
            {
                ErrorType.Validation => BadRequest(new { result.Error }),
                ErrorType.Conflict => Problem(result.Error, statusCode: 409),
                _ => Problem(result.Error, statusCode: 500),
            };
        }
        return CreatedAtRoute("GetGroupUserAssignmentById", new { id = result.Value!.Id }, result.Value);
    }

    [HttpDelete("{groupUserAssignId:guid}")]
    public async Task<IActionResult> Delete(Guid groupUserAssignId, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(groupUserAssignId, ct);
        return result.IsSuccess ? NoContent() : Problem(result.Error, statusCode: 404);
    }
}
