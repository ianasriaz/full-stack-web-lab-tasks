namespace Ecommerce.Application.Dtos;

public sealed record OrderItemDto(
    int ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice);

public sealed record OrderDto(
    int Id,
    string CustomerName,
    DateTime CreatedUtc,
    decimal Total,
    IReadOnlyList<OrderItemDto> Items);

public sealed record CreateOrderItemRequest(int ProductId, int Quantity);

public sealed record CreateOrderRequest(
    string CustomerName,
    IReadOnlyList<CreateOrderItemRequest> Items);
