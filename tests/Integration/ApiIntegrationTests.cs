
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ApiForge.Application.Products.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ApiForge.Tests.Integration;

public sealed class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        // The in-memory repository is registered by default in the main project's
        // non-Development environment configuration, so we don't need to swap it out.
        _client = factory.CreateClient();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task PostProduct_WithValidData_Returns201Created()
    {
        // Arrange
        var request = new CreateProductRequest("Test Product", "A great product", 99.99m, 100);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/products", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdProduct = await response.Content.ReadFromJsonAsync<ProductResponse>();
        createdProduct.Should().NotBeNull();
        createdProduct!.Name.Should().Be(request.Name);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.OriginalString.Should().Be($"/api/v1/products/{createdProduct.Id}");
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task PostProduct_WithInvalidData_Returns400BadRequest()
    {
        // Arrange
        var request = new CreateProductRequest("", "", -10, -1); // Invalid data

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/products", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task PostProduct_WithDuplicateName_Returns409Conflict()
    {
        // Arrange
        var request = new CreateProductRequest("Unique Product Name", "A great product", 99.99m, 100);
        await _client.PostAsJsonAsync("/api/v1/products", request);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/products", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetProducts_Returns200OK_And_ListOfProducts()
    {
        // Arrange
        // Ensure there's at least one product
        var createRequest = new CreateProductRequest("Another Test Product", "Another great product", 19.99m, 50);
        await _client.PostAsJsonAsync("/api/v1/products", createRequest);

        // Act
        var response = await _client.GetAsync("/api/v1/products");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pagedResult = await response.Content.ReadFromJsonAsync<JsonDocument>(); // Use JsonDocument to parse dynamic structure
        pagedResult.Should().NotBeNull();
        var items = pagedResult!.RootElement.GetProperty("items").EnumerateArray().ToList();
        items.Should().NotBeEmpty();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetProductById_WhenFound_Returns200OK()
    {
        // Arrange
        var createRequest = new CreateProductRequest("Find Me Product", "A specific product", 49.99m, 10);
        var createResponse = await _client.PostAsJsonAsync("/api/v1/products", createRequest);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductResponse>();
        var productId = createdProduct!.Id;

        // Act
        var response = await _client.GetAsync($"/api/v1/products/{productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetchedProduct = await response.Content.ReadFromJsonAsync<ProductResponse>();
        fetchedProduct.Should().NotBeNull();
        fetchedProduct!.Id.Should().Be(productId);
        fetchedProduct.Name.Should().Be(createRequest.Name);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetProductById_WhenNotFound_Returns404NotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/v1/products/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task PutProduct_WithValidData_Returns200OK()
    {
        // Arrange
        var createRequest = new CreateProductRequest("Updatable Product", "Initial state", 10m, 10);
        var createResponse = await _client.PostAsJsonAsync("/api/v1/products", createRequest);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductResponse>();
        var productId = createdProduct!.Id;
        var updateRequest = new UpdateProductRequest("Updated Product Name", "Updated state", 20m, 20);

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/products/{productId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedProduct = await response.Content.ReadFromJsonAsync<ProductResponse>();
        updatedProduct.Should().NotBeNull();
        updatedProduct!.Name.Should().Be(updateRequest.Name);
        updatedProduct.Price.Should().Be(updateRequest.Price);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task PutProduct_WhenNotFound_Returns404NotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var updateRequest = new UpdateProductRequest("Non-existent", "...", 1, 1);

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/products/{nonExistentId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task PutProduct_WithInvalidData_Returns400BadRequest()
    {
        // Arrange
        var createRequest = new CreateProductRequest("Product To Update Invalidly", "Initial state", 10m, 10);
        var createResponse = await _client.PostAsJsonAsync("/api/v1/products", createRequest);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductResponse>();
        var productId = createdProduct!.Id;
        var updateRequest = new UpdateProductRequest("", "", -1, -1); // Invalid data

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/products/{productId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task DeleteProduct_WhenFound_Returns204NoContent()
    {
        // Arrange
        var createRequest = new CreateProductRequest("Product To Delete", "Will be deleted", 5m, 5);
        var createResponse = await _client.PostAsJsonAsync("/api/v1/products", createRequest);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductResponse>();
        var productId = createdProduct!.Id;

        // Act
        var response = await _client.DeleteAsync($"/api/v1/products/{productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify it's gone
        var getResponse = await _client.GetAsync($"/api/v1/products/{productId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task DeleteProduct_WhenNotFound_Returns404NotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/products/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
