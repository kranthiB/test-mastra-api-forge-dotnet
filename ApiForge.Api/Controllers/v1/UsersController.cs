
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

    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), 201)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _userService.CreateAsync(request, cancellationToken);

        if (result.IsSuccess)
        {
            var user = result.Value!;
            return CreatedAtRoute("GetUserById", new { id = user.Id, version = "1.0" }, user);
        }

        return result.ErrorKind switch
        {
            ErrorType.Validation => BadRequest(new { result.Error }),
            ErrorType.Conflict => Problem(result.Error, statusCode: StatusCodes.Status409Conflict),
            _ => Problem(result.Error, statusCode: StatusCodes.Status500InternalServerError),
        };
    }

    [HttpGet("{id:guid}", Name = "GetUserById")]
    public Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    public Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    [HttpPut("{id:guid}")]
    public Task<IActionResult> Update(Guid id, [FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id:guid}")]
    public Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
