using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.Extensions.Logging;

namespace Microsoft.eShopWeb.Infrastructure.Services;
internal sealed class OrderCreatedEventSender(HttpClient httpClient, ILogger<OrderCreatedEventSender> logger) : IOrderCreatedEventSender
{
    private sealed record CreatedOrder(int OrderId, decimal FinalPrice, Address ShipToAddress, IReadOnlyCollection<OrderItem> items);

    async public Task SendAsync(Order order)
    {
        CreatedOrder details = new(order.Id, order.Total(), order.ShipToAddress, order.OrderItems);
        string payload = JsonSerializer.Serialize(details, JsonSerializerOptions.Web);
        logger.LogInformation(payload);
        var content = new StringContent(payload, null, "application/json");
        var response = await httpClient.PostAsync("DeliveryOrderProcessor", content);
        response.EnsureSuccessStatusCode();
    }
}
