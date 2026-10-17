using Ecommerce.Application.Dtos;

namespace Ecommerce.Application.Abstractions;

public interface IProductApiClient
{
    Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductDto>> GetProductsAsync(CancellationToken cancellationToken = default);

    Task<ProductDto?> GetProductAsync(int id, CancellationToken cancellationToken = default);

    Task<ProductDto> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken = default);

    Task UpdateProductAsync(int id, UpdateProductRequest request, CancellationToken cancellationToken = default);

    Task DeleteProductAsync(int id, CancellationToken cancellationToken = default);
}
