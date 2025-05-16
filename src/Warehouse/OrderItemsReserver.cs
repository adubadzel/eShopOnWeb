using Ardalis.GuardClauses;
using Microsoft.Azure.Functions.Worker;

namespace Warehouse;

public class OrderItemsReserver
{
    public record Order(int OrderId, OrderItem[] OrderItems);
    public record OrderItem(int CatalogItemId, int Quantity);

    [Function(nameof(OrderItemsReserver))]
    [BlobOutput("order-items-reservation/{OrderId}")]
    public OrderItem[] Run([ServiceBusTrigger("reserve-for-order", Connection = "ServiceBusConnection")] Order order)
    {
        // TODO validate, transform
        Guard.Against.Null(order);
        return order.OrderItems;
    }
}
