
using ApiForge.Application.Groups.DTOs;
using ApiForge.Application.Groups.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using Swashbuckle.AspNetCore.Annotations;
using ApiForge.Application.Common.Models;
using Microsoft.AspNetCore.Http;

namespace ApiForge.Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/groups")]
[Produces("application/json")]
[SwaggerTag("Groups")]
public sealed class GroupsController : ControllerBase
{
    private readonly IGroupService _service;

    public GroupsController(IGroupService service) => _service = service;

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new group", OperationId = "Groups_Create")]
    [ProducesResponseType(typeof(GroupResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateGroupRequest request, CancellationToken ct)
    {
        var result = await _service.CreateAsync(request, ct);

        if (!result.IsSuccess)
        {
            return result.ErrorKind switch
            {
                ErrorType.Validation => BadRequest(new { errors = result.Error }),
                ErrorType.Conflict => Problem(result.Error, statusCode: StatusCodes.Status409Conflict),
                _ => Problem(result.Error, statusCode: StatusCodes.Status500InternalServerError),
            };
        }

        var group = result.Value!;
        return CreatedAtRoute("GetGroupById", new { id = group.Id, version = "1.0" }, group);
    }

    [HttpGet("{id:guid}", Name = "GetGroupById")]
    [SwaggerOperation(Summary = "Get a group by its ID", OperationId = "Groups_GetById")]
    [ProducesResponseType(typeof(GroupResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpGet]
    [SwaggerOperation(Summary = "List groups (paged)", OperationId = "Groups_List")]
    [ProducesResponseType(typeof(OffsetPagedResult<GroupResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int offset = 0,
        [FromQuery] int limit = 25,
        CancellationToken ct = default)
    {
        if (limit <= 0)
        {
            return BadRequest(new ProblemDetails { Title = "Invalid limit", Detail = "limit parameter must be greater than 0." });
        }

        if (limit > 1000)
        {
            return BadRequest(new ProblemDetails { Title = "Invalid limit", Detail = "limit parameter cannot be greater than 1000." });
        }

        if (offset < 0)
        {
            return BadRequest(new ProblemDetails { Title = "Invalid offset", Detail = "offset parameter cannot be negative." });
        }

        var result = await _service.GetAllAsync(offset, limit, ct);
        return Ok(result.Value);
    }

    [HttpGet("{groupSlug}", Name = "GetGroupBySlug")]
    [SwaggerOperation(Summary = "Get a group by its slug", OperationId = "Groups_GetBySlug")]
    [ProducesResponseType(typeof(GroupResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBySlug([FromRoute] string groupSlug, CancellationToken ct)
    {
        var result = await _service.GetBySlugAsync(groupSlug, ct);
        if (!result.IsSuccess)
        {
            return Problem(result.Error, statusCode: StatusCodes.Status404NotFound);
        }

        return Ok(result.Value);
    }
}
