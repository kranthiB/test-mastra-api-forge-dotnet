
using ApiForge.Application.Common.Models;
using ApiForge.Application.Users.DTOs;
using ApiForge.Application.Users.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
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

    [HttpGet(Name = "GetUsers")]
    [ProducesResponseType(typeof(PaginatedResponseDto<UserResponse>), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    public async Task<IActionResult> GetAll([FromQuery] int offset = 0, [FromQuery] int limit = 25, CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetAllAsync(offset, limit, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ErrorKind switch
            {
                ErrorType.Validation => BadRequest(new { result.Error }),
                _ => Problem(result.Error, statusCode: 500),
            };
        }

        var paginatedResult = result.Value!;
        AddPaginationLinks(paginatedResult);

        return Ok(paginatedResult);
    }

    private void AddPaginationLinks(PaginatedResponseDto<UserResponse> result)
    {
        result.Links["self"] = Url.Link("GetUsers", new { offset = result.Offset, limit = result.Limit })!;

        if (result.Offset > 0)
        {
            var prevOffset = Math.Max(0, result.Offset - result.Limit);
            result.Links["prev"] = Url.Link("GetUsers", new { offset = prevOffset, limit = result.Limit })!;
        }

        if (result.Offset + result.Limit < result.Total)
        {
            var nextOffset = result.Offset + result.Limit;
            result.Links["next"] = Url.Link("GetUsers", new { offset = nextOffset, limit = result.Limit })!;
        }
    }

    // Other methods from the original file are kept below
    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), 201)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public Task<IActionResult> Create([FromBody] object request, CancellationToken cancellationToken) // Using object for placeholder
    {
        return Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status501NotImplemented));
    }

    [HttpGet("{id:guid}", Name = "GetUserById")]
    [ProducesResponseType(typeof(UserResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _userService.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            return result.ErrorKind switch
            {
                ErrorType.NotFound => Problem(result.Error, statusCode: StatusCodes.Status404NotFound),
                _ => Problem(result.Error, statusCode: StatusCodes.Status500InternalServerError)
            };
        }
        return Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UserResponse), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateAsync(id, request, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ErrorKind switch
            {
                ErrorType.Validation => BadRequest(new { result.Error }),
                ErrorType.NotFound => Problem(result.Error, statusCode: StatusCodes.Status404NotFound),
                ErrorType.Conflict => Problem(result.Error, statusCode: StatusCodes.Status409Conflict),
                _ => Problem(result.Error, statusCode: StatusCodes.Status500InternalServerError),
            };
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status501NotImplemented));
    }
}
