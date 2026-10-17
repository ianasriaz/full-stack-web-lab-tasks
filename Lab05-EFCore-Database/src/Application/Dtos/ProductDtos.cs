namespace Ecommerce.Application.Dtos;

public sealed record ProductDto(
    int Id,
    string Name,
    string Description,
    decimal Price,
    int Stock,
    int CategoryId,
    string CategoryName);

public sealed record CreateProductRequest(
    string Name,
    string Description,
    decimal Price,
    int Stock,
    int CategoryId);

public sealed record UpdateProductRequest(
    string Name,
    string Description,
    decimal Price,
    int Stock,
    int CategoryId);
