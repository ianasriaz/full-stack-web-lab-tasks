using Ecommerce.Application.Abstractions;
using Ecommerce.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly EcommerceDbContext _dbContext;

    public ProductRepository(EcommerceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Products
            .Include(product => product.Category)
            .AsNoTracking()
            .OrderBy(product => product.Id)
            .ToListAsync(cancellationToken);

    public Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _dbContext.Products
            .Include(product => product.Category)
            .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);

    public async Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<bool> UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _dbContext.Products.Update(product);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _dbContext.Products.FindAsync([id], cancellationToken);
        if (product is null)
        {
            return false;
        }

        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
