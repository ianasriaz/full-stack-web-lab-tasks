using Ecommerce.Domain;

namespace Ecommerce.Application.Abstractions;

public interface IOrderRepository
{
    Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default);
}
