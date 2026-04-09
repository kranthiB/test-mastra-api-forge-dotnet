
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
}
