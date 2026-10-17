using Ecommerce.Domain;

namespace Ecommerce.Application.Abstractions;

public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
