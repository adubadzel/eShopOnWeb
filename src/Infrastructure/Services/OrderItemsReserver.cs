using System;
using System.Linq;
using System.Threading.Tasks;
using Azure.Messaging;
using Azure.Messaging.ServiceBus;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.Infrastructure.Services;

internal sealed class OrderItemsReserver(ServiceBusClient serviceBusClient) : IOrderItemsReserver
{
    private readonly ServiceBusSender queueSender = serviceBusClient.CreateSender("reserve-for-order");

    private sealed record OrderReservation(int OrderId, OrderItemReservation[] OrderItems);
    private sealed record OrderItemReservation(int CatalogItemId, int Quantity);
    public async Task ReserveFor(Order order)
    {
        OrderReservation details = new(order.Id, [.. order.OrderItems.Select(i => new OrderItemReservation(i.ItemOrdered.CatalogItemId, i.Units))]);
        var cloudEvent = new CloudEvent(
            "/eshoponweb/web",
            "eShopWeb.OrderReservation",
            details);
        await queueSender.SendMessageAsync(new ServiceBusMessage(new BinaryData(cloudEvent))
        {
            ContentType = "application/cloudevents+json"
        });
    }
}
