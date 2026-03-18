using ApiForge.Application.Common.Models;
using ApiForge.Application.Products.DTOs;
using ApiForge.Application.Products.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ApiForge.Api.Controllers.v1;

/// <summary>
/// Reference CRUD implementation for the <c>Product</c> resource.
/// Copy this controller as a template when adding new resources.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
[SwaggerTag("Products – full CRUD example for the team template")]
public sealed class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service) => _service = service;

    // ── GET /api/v1/products ───────────────────────────────────────────────

    /// <summary>Returns a paginated list of products.</summary>
    [HttpGet]
    [SwaggerOperation(Summary = "List products (paged)", OperationId = "Products_List")]
    [ProducesResponseType(typeof(PagedResult<ProductResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int    page     = 1,
        [FromQuery] int    pageSize = 10,
        [FromQuery] bool?  isActive = null,
        CancellationToken  ct       = default)
    {
        var result = await _service.GetAllAsync(page, pageSize, isActive, ct);
        return Ok(result.Value);
    }

    // ── GET /api/v1/products/{id} ──────────────────────────────────────────

    /// <summary>Returns a single product by its ID.</summary>
    [HttpGet("{id:guid}", Name = "GetProductById")]
    [SwaggerOperation(Summary = "Get a product by ID", OperationId = "Products_GetById")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),  StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : Problem(result.Error, statusCode: StatusCodes.Status404NotFound);
    }

    // ── POST /api/v1/products ──────────────────────────────────────────────

    /// <summary>Creates a new product.</summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a product", OperationId = "Products_Create")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails),           StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateProductRequest request,
        CancellationToken ct)
    {
        var result = await _service.CreateAsync(request, ct);

        if (!result.IsSuccess)
            return result.ErrorKind switch
            {
                ErrorType.Validation => ValidationProblem(result.Error!),
                ErrorType.Conflict   => Problem(result.Error, statusCode: StatusCodes.Status409Conflict),
                _                    => Problem(result.Error, statusCode: StatusCodes.Status400BadRequest),
            };

        return CreatedAtRoute(
            "GetProductById",
            new { id = result.Value!.Id },
            result.Value);
    }

    // ── PUT /api/v1/products/{id} ──────────────────────────────────────────

    /// <summary>Replaces all mutable fields of an existing product.</summary>
    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Update a product", OperationId = "Products_Update")]
    [ProducesResponseType(typeof(ProductResponse),          StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails),           StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails),           StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateProductRequest request,
        CancellationToken ct)
    {
        var result = await _service.UpdateAsync(id, request, ct);

        if (!result.IsSuccess)
            return result.ErrorKind switch
            {
                ErrorType.NotFound   => Problem(result.Error, statusCode: StatusCodes.Status404NotFound),
                ErrorType.Conflict   => Problem(result.Error, statusCode: StatusCodes.Status409Conflict),
                ErrorType.Validation => ValidationProblem(result.Error!),
                _                    => Problem(result.Error, statusCode: StatusCodes.Status400BadRequest),
            };

        return Ok(result.Value);
    }

    // ── DELETE /api/v1/products/{id} ───────────────────────────────────────

    /// <summary>Permanently removes a product.</summary>
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Delete a product", OperationId = "Products_Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(id, ct);
        return result.IsSuccess
            ? NoContent()
            : Problem(result.Error, statusCode: StatusCodes.Status404NotFound);
    }

    // ── Helper ─────────────────────────────────────────────────────────────

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
