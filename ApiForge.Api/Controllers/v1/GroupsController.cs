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

    public GroupsController(IGroupService groupService)
    {
        _groupService = groupService;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new group", OperationId = "Groups_Create")]
    [ProducesResponseType(typeof(GroupResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateGroupRequest request, CancellationToken ct)
    {
        var result = await _groupService.CreateAsync(request, ct);

        if (result.IsSuccess)
            return CreatedAtRoute("GetGroupById", new { id = result.Value!.Id, version = "1.0" }, result.Value);

        return result.ErrorKind switch
        {
            ErrorType.Validation => BadRequest(new { errors = result.Error }),
            ErrorType.Conflict => Problem(result.Error, statusCode: StatusCodes.Status409Conflict),
            _ => Problem(result.Error, statusCode: StatusCodes.Status500InternalServerError),
        };
    }

    [HttpGet("{id:guid}", Name = "GetGroupById")]
    [SwaggerOperation(Summary = "Get a group by its unique ID", OperationId = "Groups_GetById")]
    [ProducesResponseType(typeof(GroupResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _groupService.GetByIdAsync(id, ct);
        return result.IsSuccess ? Ok(result.Value) : Problem(result.Error, statusCode: 404);
    }

    [HttpGet]
    [SwaggerOperation(Summary = "List groups (paged)", OperationId = "Groups_List")]
    [ProducesResponseType(typeof(PagedResult<GroupResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await _groupService.GetAllAsync(page, pageSize, ct);
        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Delete a group by its unique ID", OperationId = "Groups_Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _groupService.DeleteAsync(id, ct);
        return result.IsSuccess ? NoContent() : Problem(result.Error, statusCode: 404);
    }

    [HttpDelete("{groupSlug}")]
    [SwaggerOperation(Summary = "Delete a group by its unique slug", OperationId = "Groups_DeleteBySlug")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBySlug([FromRoute] string groupSlug, CancellationToken ct)
    {
        var result = await _groupService.DeleteBySlugAsync(groupSlug, ct);
        return result.IsSuccess ? NoContent() : Problem(result.Error, statusCode: 404);
    }

    [HttpGet("{groupSlug}", Name = "GetGroupBySlug")]
    [SwaggerOperation(Summary = "Get a group by its unique slug", OperationId = "Groups_GetBySlug")]
    [ProducesResponseType(typeof(GroupResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBySlug([FromRoute] string groupSlug, CancellationToken ct)
    {
        var result = await _groupService.GetBySlugAsync(groupSlug, ct);
        return result.IsSuccess ? Ok(result.Value) : Problem(result.Error, statusCode: 404);
    }

    [HttpPut("{groupSlug}")]
    [SwaggerOperation(Summary = "Update a group by its unique slug", OperationId = "Groups_Update")]
    [ProducesResponseType(typeof(GroupResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update([FromRoute] string groupSlug, [FromBody] UpdateGroupRequest request, CancellationToken ct)
    {
        var result = await _groupService.UpdateAsync(groupSlug, request, ct);

        if (result.IsSuccess)
            return Ok(result.Value);

        return result.ErrorKind switch
        {
            ErrorType.Validation => BadRequest(new { errors = result.Error }),
            ErrorType.NotFound => Problem(result.Error, statusCode: StatusCodes.Status404NotFound),
            ErrorType.Conflict => Problem(result.Error, statusCode: StatusCodes.Status409Conflict),
            _ => Problem(result.Error, statusCode: StatusCodes.Status500InternalServerError),
        };
    }
}
