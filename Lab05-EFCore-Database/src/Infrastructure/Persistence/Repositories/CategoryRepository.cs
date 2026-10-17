using Ecommerce.Application.Abstractions;
using Ecommerce.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly EcommerceDbContext _dbContext;

    public CategoryRepository(EcommerceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Categories
            .AsNoTracking()
            .OrderBy(category => category.Id)
            .ToListAsync(cancellationToken);

    public Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _dbContext.Categories.FirstOrDefaultAsync(category => category.Id == id, cancellationToken);
}
