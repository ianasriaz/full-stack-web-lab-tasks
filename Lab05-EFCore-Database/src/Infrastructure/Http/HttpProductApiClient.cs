using System.Net;
using System.Net.Http.Json;
using Ecommerce.Application.Abstractions;
using Ecommerce.Application.Dtos;

namespace Ecommerce.Infrastructure.Http;

public sealed class HttpProductApiClient : IProductApiClient
{
    private readonly HttpClient _httpClient;

    public HttpProductApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken = default) =>
        await _httpClient.GetFromJsonAsync<List<CategoryDto>>("api/categories", cancellationToken)
            ?? new List<CategoryDto>();

    public async Task<IReadOnlyList<ProductDto>> GetProductsAsync(CancellationToken cancellationToken = default) =>
        await _httpClient.GetFromJsonAsync<List<ProductDto>>("api/products", cancellationToken)
            ?? new List<ProductDto>();

    public Task<ProductDto?> GetProductAsync(int id, CancellationToken cancellationToken = default) =>
        _httpClient.GetFromJsonAsync<ProductDto>($"api/products/{id}", cancellationToken);

    public async Task<ProductDto> CreateProductAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/products", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ProductDto>(cancellationToken)
            ?? throw new InvalidOperationException("The API returned an empty product.");
    }

    public async Task UpdateProductAsync(
        int id,
        UpdateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/products/{id}", request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteProductAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"api/products/{id}", cancellationToken);
        if (response.StatusCode != HttpStatusCode.NotFound)
        {
            response.EnsureSuccessStatusCode();
        }
    }
}
