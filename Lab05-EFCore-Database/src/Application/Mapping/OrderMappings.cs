using Ecommerce.Application.Dtos;
using Ecommerce.Domain;

namespace Ecommerce.Application.Mapping;

public static class OrderMappings
{
    public static OrderDto ToDto(this Order order) =>
        new(
            order.Id,
            order.CustomerName,
            order.CreatedUtc,
            order.Total,
            order.Items
                .Select(item => new OrderItemDto(
                    item.ProductId,
                    item.Product?.Name ?? string.Empty,
                    item.Quantity,
                    item.UnitPrice))
                .ToList());
}
