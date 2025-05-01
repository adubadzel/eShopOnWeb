using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Microsoft.eShopWeb.Infrastructure.Services;

internal sealed class OrderItemsReserver(HttpClient httpClient) : IOrderItemsReserver
{
    private sealed record OrderReservation(int OrderId, OrderItemReservation[] OrderItems);
    private sealed record OrderItemReservation(int CatalogItemId, int Quantity);
    public async Task ReserveFor(Order order)
    {
        OrderReservation details = new(order.Id, [.. order.OrderItems.Select(i => new OrderItemReservation(i.ItemOrdered.CatalogItemId, i.Units))]);
        string payload = JsonSerializer.Serialize(details, JsonSerializerOptions.Web);
        var content = new StringContent(payload, null, "application/json");
        var response = await httpClient.PostAsync("OrderItemsReserver", content);
        response.EnsureSuccessStatusCode();
    }
}

internal sealed class OrderItemsReserverAuthHandler(IConfiguration configuration) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Add("x-functions-key", configuration["warehouseapi:key"]);
        return await base.SendAsync(request, cancellationToken);
    }
}
