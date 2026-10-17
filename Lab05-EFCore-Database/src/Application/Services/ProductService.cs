using Ecommerce.Application.Abstractions;
using Ecommerce.Application.Dtos;
using Ecommerce.Application.Mapping;
using Ecommerce.Domain;

namespace Ecommerce.Application.Services;

public sealed class ProductService
{
    private readonly IProductRepository _products;
    private readonly ICategoryRepository _categories;

    public ProductService(IProductRepository products, ICategoryRepository categories)
    {
        _products = products;
        _categories = categories;
    }

    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await _products.GetAllAsync(cancellationToken);
        return products.Select(product => product.ToDto()).ToList();
    }

    public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _products.GetByIdAsync(id, cancellationToken);
        return product?.ToDto();
    }

    public async Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        Validate(request.Name, request.Price, request.Stock);

        var category = await _categories.GetByIdAsync(request.CategoryId, cancellationToken)
            ?? throw new ArgumentException($"Category {request.CategoryId} does not exist.");

        var product = new Product
        {
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            Price = request.Price,
            Stock = request.Stock,
            CategoryId = category.Id,
            Category = category
        };

        await _products.AddAsync(product, cancellationToken);
        return product.ToDto();
    }

    public async Task<bool> UpdateAsync(int id, UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        Validate(request.Name, request.Price, request.Stock);

        var product = await _products.GetByIdAsync(id, cancellationToken);
        if (product is null)
        {
            return false;
        }

        var category = await _categories.GetByIdAsync(request.CategoryId, cancellationToken)
            ?? throw new ArgumentException($"Category {request.CategoryId} does not exist.");

        product.Name = request.Name.Trim();
        product.Description = request.Description.Trim();
        product.Price = request.Price;
        product.Stock = request.Stock;
        product.CategoryId = category.Id;
        product.Category = category;

        return await _products.UpdateAsync(product, cancellationToken);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        _products.DeleteAsync(id, cancellationToken);

    private static void Validate(string name, decimal price, int stock)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Product name is required.");
        }

        if (price <= 0)
        {
            throw new ArgumentException("Price must be greater than zero.");
        }

        if (stock < 0)
        {
            throw new ArgumentException("Stock cannot be negative.");
        }
    }
}
