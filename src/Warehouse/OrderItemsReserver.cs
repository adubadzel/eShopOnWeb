using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Warehouse;

public record Order(int OrderId, OrderItem[] OrderItems);
public record OrderItem(int CatalogItemId, int Quantity);

public class OrderItemsReserver
{
    [Function(nameof(OrderItemsReserver))]
    [BlobOutput("order-items-reservation/{OrderId}")]
    public OrderItem[] Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req, [FromBody] Order order)
    {
        // TODO validate, transform
        return order.OrderItems;
    }
}
