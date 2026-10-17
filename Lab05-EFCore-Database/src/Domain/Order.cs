namespace Ecommerce.Domain;

public class Order
{
    public int Id { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public DateTime CreatedUtc { get; set; }

    public decimal Total { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
