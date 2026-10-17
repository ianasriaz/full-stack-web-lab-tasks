using Ecommerce.Application.Abstractions;
using Ecommerce.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly EcommerceDbContext _dbContext;

    public OrderRepository(EcommerceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Orders
            .Include(order => order.Items)
            .ThenInclude(item => item.Product)
            .AsNoTracking()
            .OrderBy(order => order.Id)
            .ToListAsync(cancellationToken);

    public async Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return order;
    }
}
