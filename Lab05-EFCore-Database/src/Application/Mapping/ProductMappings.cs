using Ecommerce.Application.Dtos;
using Ecommerce.Domain;

namespace Ecommerce.Application.Mapping;

public static class ProductMappings
{
    public static ProductDto ToDto(this Product product) =>
        new(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.Stock,
            product.CategoryId,
            product.Category?.Name ?? string.Empty);
}
