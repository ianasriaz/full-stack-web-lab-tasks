using Ecommerce.Application.Abstractions;
using Ecommerce.Application.Dtos;
using Ecommerce.Application.Mapping;

namespace Ecommerce.Application.Services;

public sealed class CategoryService
{
    private readonly ICategoryRepository _categories;

    public CategoryService(ICategoryRepository categories)
    {
        _categories = categories;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categories.GetAllAsync(cancellationToken);
        return categories.Select(category => category.ToDto()).ToList();
    }
}
