using Ecommerce.Application.Dtos;
using Ecommerce.Domain;

namespace Ecommerce.Application.Mapping;

public static class CategoryMappings
{
    public static CategoryDto ToDto(this Category category) =>
        new(category.Id, category.Name, category.Slug);
}
