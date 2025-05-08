using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;

namespace Warehouse;

public class DeliveryOrderProcessor
{
    public sealed record CreatedOrder(string id, int orderId, decimal finalPrice, Address shipToAddress, IReadOnlyCollection<OrderItem> items);

    [Function("DeliveryOrderProcessor")]
    [CosmosDBOutput("Delivery", "created-orders", Connection = "CosmosDbConnection")]
    public CreatedOrder Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req, [FromBody] CreatedOrder order)
    {
         return order with { id = Guid.NewGuid().ToString() };
    }
}
