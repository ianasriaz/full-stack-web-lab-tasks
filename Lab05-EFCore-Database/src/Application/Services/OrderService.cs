using Ecommerce.Application.Abstractions;
using Ecommerce.Application.Dtos;
using Ecommerce.Application.Mapping;
using Ecommerce.Domain;

namespace Ecommerce.Application.Services;

public sealed class OrderService
{
    private readonly IOrderRepository _orders;
    private readonly IProductRepository _products;

    public OrderService(IOrderRepository orders, IProductRepository products)
    {
        _orders = orders;
        _products = products;
    }

    public async Task<IReadOnlyList<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var orders = await _orders.GetAllAsync(cancellationToken);
        return orders.Select(order => order.ToDto()).ToList();
    }

    public async Task<OrderDto> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.CustomerName))
        {
            throw new ArgumentException("Customer name is required.");
        }

        if (request.Items is null || request.Items.Count == 0)
        {
            throw new ArgumentException("An order needs at least one item.");
        }

        var order = new Order
        {
            CustomerName = request.CustomerName.Trim(),
            CreatedUtc = DateTime.UtcNow
        };

        var total = 0m;

        foreach (var line in request.Items)
        {
            if (line.Quantity <= 0)
            {
                throw new ArgumentException("Item quantity must be greater than zero.");
            }

            var product = await _products.GetByIdAsync(line.ProductId, cancellationToken)
                ?? throw new ArgumentException($"Product {line.ProductId} does not exist.");

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                Product = product,
                Quantity = line.Quantity,
                UnitPrice = product.Price
            });

            total += product.Price * line.Quantity;
        }

        order.Total = total;

        await _orders.AddAsync(order, cancellationToken);
        return order.ToDto();
    }
}
