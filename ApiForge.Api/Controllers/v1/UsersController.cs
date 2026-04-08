
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
    [SwaggerOperation(Summary = "List users (paged)", OperationId = "Users_List")]
    [ProducesResponseType(typeof(OffsetPagedResult<UserResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int offset = 0,
        [FromQuery] int limit = 25,
        CancellationToken ct = default)
    {
        var result = await _userService.GetAllAsync(offset, limit, ct);
        return Ok(result.Value);
    }
}
