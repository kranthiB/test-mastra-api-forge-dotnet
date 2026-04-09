
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
    private readonly IUserService _service;

    public UsersController(IUserService service) => _service = service;

    [HttpGet]
    [SwaggerOperation(Summary = "List users (paged)", OperationId = "Users_List")]
    [ProducesResponseType(typeof(PagedResult<UserResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        var result = await _service.GetAllAsync(page, pageSize, ct);
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}", Name = "GetUserById")]
    [SwaggerOperation(Summary = "Get a user by ID", OperationId = "Users_GetById")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : Problem(result.Error, statusCode: StatusCodes.Status404NotFound);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a user", OperationId = "Users_Create")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        var result = await _service.CreateAsync(request, ct);

        if (!result.IsSuccess)
            return result.ErrorKind switch
            {
                ErrorType.Validation => BadRequest(new { result.Error }),
                ErrorType.Conflict => Problem(result.Error, statusCode: StatusCodes.Status409Conflict),
                _ => Problem(result.Error, statusCode: StatusCodes.Status500InternalServerError),
            };

        return CreatedAtRoute("GetUserById", new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Update a user", OperationId = "Users_Update")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken ct)
    {
        var result = await _service.UpdateAsync(id, request, ct);

        if (!result.IsSuccess)
            return result.ErrorKind switch
            {
                ErrorType.NotFound => Problem(result.Error, statusCode: StatusCodes.Status404NotFound),
                ErrorType.Conflict => Problem(result.Error, statusCode: StatusCodes.Status409Conflict),
                ErrorType.Validation => BadRequest(new { result.Error }),
                _ => Problem(result.Error, statusCode: StatusCodes.Status500InternalServerError),
            };

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Delete a user", OperationId = "Users_Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(id, ct);
        if (result.IsSuccess) return NoContent();

        return result.ErrorKind switch
        {
            ErrorType.NotFound => Problem(result.Error, statusCode: StatusCodes.Status404NotFound),
            ErrorType.Conflict => Problem(result.Error, statusCode: StatusCodes.Status409Conflict),
            _ => Problem(result.Error, statusCode: StatusCodes.Status500InternalServerError),
        };
    }
}
